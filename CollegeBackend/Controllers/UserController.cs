using CollegeBackend.Auth;
using CollegeBackend.Extensions;
using CollegeBackend.Objects;
using CollegeBackend.Objects.Database;
using CollegeBackend.Objects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("users/[controller]")]
[ApiController]
public class UserController : Controller
{
    private const string SpecialChars = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-";

    private readonly CollegeBackendContext _context;
    private readonly IPasswordHasher<User?> _passwordHasher;
    private readonly IAuthenticationManager _authenticationManager;

    public UserController(CollegeBackendContext context, IPasswordHasher<User?> passwordHasher,
        IAuthenticationManager authenticationManager)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _authenticationManager = authenticationManager;
    }

    [HttpGet("deleteUser")]
    [Authorize(Policy = "AdministratorAndModerator")]
    public async Task<JsonResult> DeleteUser([FromBody] DeleteUserModel deleteUserModel)
    {
        return new JsonResult("");
    }

    [HttpGet("updateRole")]
    [Authorize(Policy = "Administrator")]
    public async Task<JsonResult> UpdateRole(
        [FromBody] RoleUpdateModel roleUpdateModel)
    {
        return new JsonResult("");
    }

    [HttpGet("loginUser")]
    [AllowAnonymous]
    public async Task<JsonResult> Login(
        [FromBody] LoginModel loginModel)
    {
        // try to generate new toke
        var result = await GenerateTokenAsync(loginModel);

        // if not success, return error message from result
        if (result.NotSuccess) return result.ErrorMessage.ToActionResult();

        // else ensure that result is not null
        var user = result.Result ?? throw new Exception();

        // return token as string
        return user.Token.ToString().ToActionResult();
    }

    [HttpGet("registerUser")]
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

            await _context.Users.AddAsync(new User
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
            });

            return UserEnumRegisterResult.Success.ToActionResult();
        }
        catch (Exception) // catch any exception
        {
            // return it as result
            return UserEnumRegisterResult.InternalError.ToActionResult();
        }
    }

    private async Task<IGenericResult<TokenizedUser>> GenerateTokenAsync(IUserTargetedModel userTargetedModel)
    {
        // try to find user by name and passport id
        var user = await GetUser(userTargetedModel);

        // if user not found, return it
        if (user == null)
        {
            // by default, generic result holds failed value and null entity
            return new GenericResult<TokenizedUser>
            {
                ErrorMessage = "User not found"
            };
        }

        // verify password
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userTargetedModel.Password);

        // if password is not verified successfully, return error message
        if (result != PasswordVerificationResult.Success)
            return new GenericResult<TokenizedUser>
            {
                ErrorMessage = "Invalid password"
            };

        // try to auth with manager
        var generated = await _authenticationManager.AuthenticateAsync(user);

        // if it's null, that means, user already exists in cache
        if (generated == null)
        {
            return new GenericResult<TokenizedUser>
            {
                ErrorMessage = "Already logged in"
            };
        }

        // return success result
        return new GenericResult<TokenizedUser>
        {
            Result = generated,
            ErrorMessage = null
        };
    }

    private static bool IsPasswordStrong(string password)
    {
        return !password.Contains(' ') // is there any whitespace (shouldn't be)
               && password.Any(char.IsLower) // is any lower chars
               && password.Any(char.IsUpper) // is any upper chars
               && password.Count(rune => SpecialChars.Contains(rune)) >= 4; // at least 4 special chars
    }

    private async Task<User?> GetUser(IUserTargetedModel userTargetedModel)
    {
        return await (
            from contextUser in _context.Users
            where contextUser.Username == userTargetedModel.Username &&
                  contextUser.PassportId == userTargetedModel.PassportId
            select contextUser).FirstOrDefaultAsync();
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

public enum UserEnumLoginResult
{
    Success,
    InvalidPassword,
    UnknownUser
}

public enum UserEnumRegisterResult
{
    Success,
    UserAlreadyExists,
    PasswordIsNotStrong,
    InternalError
}