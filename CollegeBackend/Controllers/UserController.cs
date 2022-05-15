using CollegeBackend.Auth;
using CollegeBackend.Extensions;
using CollegeBackend.Objects;
using CollegeBackend.Objects.Database;
using CollegeBackend.Objects.Models;
using CollegeBackend.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("users/[controller]")]
[ApiController]
public sealed class UserController : Controller
{
    private const string SpecialChars = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-";

    private readonly ILogger<UserController> _logger;
    private readonly CollegeBackendContext _context;
    private readonly IPasswordHasher<User?> _passwordHasher;
    private readonly IAuthenticationManager _authenticationManager;

    public UserController(CollegeBackendContext context, IPasswordHasher<User?> passwordHasher,
        IAuthenticationManager authenticationManager, ILogger<UserController> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _authenticationManager = authenticationManager;
        _logger = logger;
    }

    [HttpPost("clearCache")]
    [Authorize(Roles = "User,Administrator,Moderator")]
    public JsonResult ClearCache([FromBody] ClearCacheModel model)
    {
        var token = Guid.Parse(model.Token);


        var result = _authenticationManager.ClearAuthentication(token);

        _logger.Log(LogLevel.Information, "Clearing token for {}, is success {}", model.Token, result);

        return UserEnumClearCacheResult.Success.ToActionResult();
    }

    [HttpPost("deleteUser")]
    [Authorize(Roles = "Administrator")]
    public async Task<JsonResult> DeleteUser([FromBody] DeleteUserModel deleteUserModel)
    {
        var user = await GetUser(deleteUserModel);

        if (user == null) return UserEnumDeleteResult.UnknownUser.ToActionResult();

        var token = await _authenticationManager.GetAsyncByUser(user);

        if (token != null)
        {
            _authenticationManager.ClearAuthentication(token.Token);
        }

        await _context.Users
            .Remove(user)
            .ReloadAsync();

        await _context.SaveChangesAsync();

        return UserEnumDeleteResult.Success.ToActionResult();
    }

    [HttpPost("updateRole")]
    [Authorize(Roles = "Administrator")]
    public async Task<JsonResult> UpdateRole(
        [FromBody] RoleUpdateModel roleUpdateModel)
    {
        var user = await GetUser(roleUpdateModel);

        if (user == null) return UserEnumUpdateRoleResult.UnknownUser.ToActionResult();

        if (!Roles.AllowedNames.Contains(roleUpdateModel.NewRole))
            return UserEnumUpdateRoleResult.UnknownRole.ToActionResult();

        user.Role = roleUpdateModel.NewRole;

        await _context.Users
            .Update(user)
            .ReloadAsync();

        await _context.SaveChangesAsync();

        return UserEnumUpdateRoleResult.Success.ToActionResult();
    }

    [HttpPost("loginUser")]
    [AllowAnonymous]
    public async Task<JsonResult> Login(
        [FromBody] LoginModel loginModel)
    {
        return await GetTokenAsync(loginModel);
    }

    [HttpPost("registerUser")]
    [AllowAnonymous]
    public async Task<JsonResult> Register(
        [FromBody] RegisterModel registerModel)
    {
        try
        {
            // get user
            var possibleUser = await GetUser(registerModel);

            // if it not null, return that this user already exists
            if (possibleUser != null) return UserEnumRegisterResult.UserAlreadyExists.ToActionResult();

            if (!IsPasswordStrong(registerModel.Password))
                return UserEnumRegisterResult.PasswordIsNotStrong.ToActionResult();

            var user = new User
            {
                // hash password
                Password = _passwordHasher.HashPassword(null, registerModel.Password),
                Username = registerModel.Username,
                FirstName = registerModel.FirstName,
                SecondName = registerModel.SecondName,
                Patronymic = registerModel.Patronymic,
                PassportId = registerModel.PassportId,
                // default role = user
                Role = "User"
            };

            // add user
            await _context.Users.AddAsync(user);

            // save changes
            await _context.SaveChangesAsync();

            return await GetTokenAsync(registerModel, user);
        }
        catch (Exception) // catch any exception
        {
            // return it as result
            return UserEnumRegisterResult.InternalError.ToActionResult();
        }
    }

    private async Task<JsonResult> GetTokenAsync(IUserTargetedModel userTargetedModel, User? preUser = null)
    {
        // try to generate new toke
        var result = await GenerateTokenAsync(userTargetedModel, preUser);

        _logger.Log(LogLevel.Information, "Token generation result: {} for user with passport id {} ({})",
            result.Result?.Token, result.Result?.User.PassportId,
            result.Success ? "No any error value" : result.ErrorValue);

        // if not success, return error message from result
        if (result.NotSuccess) return result.ErrorValue.ToActionResult();

        // else ensure that result is not null
        var user = result.Result ?? throw new Exception();

        // return token as string
        return new TokenResult
        {
            Token = user.Token.ToString(),
            User = user.User
        }.ToActionResult();
    }

    private async Task<IGenericResult<TokenizedUser, UserEnumTokenResult>> GenerateTokenAsync(
        IUserTargetedModel userTargetedModel,
        User? preUser = null)
    {
        // try to find user by name and passport id or using pre-defined user from cache
        var user = preUser ?? await GetUser(userTargetedModel);

        // if user not found, return it
        if (user == null)
        {
            // by default, generic result holds failed value and null entity
            return new GenericResult<TokenizedUser, UserEnumTokenResult>
            {
                ErrorValue = UserEnumTokenResult.UserNotFound
            };
        }

        // verify password
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userTargetedModel.Password);

        // if password is not verified successfully, return error message
        if (result != PasswordVerificationResult.Success)
            return new GenericResult<TokenizedUser, UserEnumTokenResult>
            {
                ErrorValue = UserEnumTokenResult.InvalidPassword
            };

        // try to auth with manager
        var generated = await _authenticationManager.AuthenticateAsync(user);

        // if it's null, that means, user already exists in cache
        if (generated == null)
        {
            return new GenericResult<TokenizedUser, UserEnumTokenResult>
            {
                ErrorValue = UserEnumTokenResult.AlreadyLoggedIn
            };
        }

        // return success result
        return new GenericResult<TokenizedUser, UserEnumTokenResult>
        {
            Result = generated
        };
    }

    private static bool IsPasswordStrong(string password)
    {
        return password.Length is >= 6 and <= 20 // password size must be between 6 and 20
               && !password.Contains(' ') // check is there any whitespace
               && password.Any(char.IsLower) // check is there any lowercase chars
               && password.Any(char.IsUpper) // check is there any uppercase chars
               && password.Count(rune => SpecialChars.Contains(rune)) <= 4; // at least 4 special chars
    }

    private async Task<User?> GetUser(IUserTargetedModel userTargetedModel)
    {
        return await (
            from contextUser in _context.Users
            where contextUser.Username == userTargetedModel.Username &&
                  contextUser.PassportId == userTargetedModel.PassportId
            select contextUser).FirstOrDefaultAsync();
    }


    private class TokenResult
    {
        public string Token { get; set; }

        public User User { get; set; }
    }
}

public enum UserEnumDeleteResult
{
    Success,
    UnknownUser
}

public enum UserEnumUpdateRoleResult
{
    Success,
    UnknownUser,
    UnknownRole
}

public enum UserEnumTokenResult
{
    AlreadyLoggedIn,
    UserNotFound,
    InvalidPassword
}

public enum UserEnumClearCacheResult
{
    Success
}

public enum UserEnumRegisterResult
{
    UserAlreadyExists,
    PasswordIsNotStrong,
    InternalError
}