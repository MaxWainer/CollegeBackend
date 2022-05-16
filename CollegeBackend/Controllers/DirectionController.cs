using CollegeBackend.Objects.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("direction/[controller]")]
[ApiController]
public sealed class DirectionController : Controller
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
        var result = await _context.Directions
            .Include(d => d.Actives)
            .ThenInclude(a => a.Train)
            .ThenInclude(t => t.Carriages)
            .ThenInclude(c => c.Sittings)
            .ThenInclude(s => s.Ticket)
            .Include(d => d.Stations)
            .AsSingleQuery()
            .ToListAsync();

        return result;
    }
}