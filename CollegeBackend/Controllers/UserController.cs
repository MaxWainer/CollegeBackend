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

    [HttpGet("registered/{passportId}")]
    public async Task<bool> IsRegistered(int passportId)
    {
        return await _context.Users.IsExists(user => user.PassportId == passportId);
    }
}

public enum UserEnumResult
{
    Success,
    InvalidPassword,
    PassportIdAlreadyExists,
}