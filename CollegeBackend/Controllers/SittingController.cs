using CollegeBackend.Objects.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("sittings/[controller]")]
[ApiController]
public sealed class SittingController : Controller
{
    private readonly CollegeBackendContext _context;

    public SittingController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpPost("list")]
    [Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<ActionResult<List<Sitting>>> ListSittings()
    {
        return await _context.Sittings.ToListAsync(); // TODO: Include
    }
    
}