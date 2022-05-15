using CollegeBackend.Extensions;
using CollegeBackend.Objects.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace CollegeBackend.Controllers;

[Route("direction/[controller]")]
[ApiController]
public sealed class DirectionController : Controller
{
    private class RenderedDirection
    {
    }

    private readonly CollegeBackendContext _context;

    public DirectionController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    //[Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<ActionResult<List<Direction>>> ListDirections()
    {
        var result = await _context.Directions
            .Include(d => d.Actives)
            .ThenInclude(a => a.Trains)
            .ThenInclude(t => t.Carriages)
            .ThenInclude(c => c.Sittings)
            .ThenInclude(s => s.Ticket)
            .Include(d => d.Stations)
            .Where(direction => direction.DirectionId == 2) // added for test
            .ToListAsync();

        return result;
    }
}