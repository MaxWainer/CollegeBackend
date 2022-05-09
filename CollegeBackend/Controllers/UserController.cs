using CollegeBackend.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CollegeBackend.Controllers;

[Route("users/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly CollegeBackendContext _context;

    public UserController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpGet("loginUser")]
    public async Task<JsonResult> TryLogin(
        [FromBody] LoginModel loginModel)
    {
        return new JsonResult("");
    }

    [HttpGet("registerUser")]
    public async Task<JsonResult> TryRegister(
        [FromBody]
        RegisterModel registerModel)
    {
        return new JsonResult("");
    }

    private async Task<bool> IsRegistered(int passportId)
    {
        return await _context.Users.IsExists(user => user.PassportId == passportId);
    }
}

public class RegisterModel
{
    public string Username { get; set; }

    public string Password { get; set; }

    public int PassportId { get; set; }
}

public class LoginModel
{
    public string Username { get; set; }

    public string Password { get; set; }
}

public enum UserEnumLoginResult
{
    Success,
    InvalidPassword,
    UnknownUsername
}

public enum UserEnumRegisterResult
{
    Success,
    UsernameAlreadyExists,
    PassportIdAlreadyExists,
    PasswordIsNotStrong
}