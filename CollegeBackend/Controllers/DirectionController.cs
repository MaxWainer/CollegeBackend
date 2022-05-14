using CollegeBackend.Objects.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("direction/[controller]")]
[ApiController]
public class DirectionController : Controller
{
    private readonly CollegeBackendContext _context;

    public DirectionController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    [Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<ActionResult<List<Direction>>> ListDirections()
    {
        return await _context.Directions
            .Include(direction => direction.Stations) // we need stations to displaying
            .Include(direction => direction.Actives) // also actives
            .ToListAsync();
    }
}