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
        return await _context.Directions
            .AsQueryable()
            .Include(direction => direction.Actives)
                .ThenInclude(active => active.Train)
                    .ThenInclude(train => train.Carriages)
                        .ThenInclude(carriage => carriage.Sittings)
                            .ThenInclude(sitting => sitting.Ticket)
            .Include(direction => direction.Stations)
            .ToListAsync();
    }
}