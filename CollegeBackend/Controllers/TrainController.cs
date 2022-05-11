using CollegeBackend.Objects.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("trains/[controller]")]
[ApiController]
public class TrainController : Controller
{
    private readonly CollegeBackendContext _context;

    public TrainController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpPost("list")]
    [Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<ActionResult<List<Train>>> ListTrains()
    {
        return await _context.Trains.ToListAsync(); // TODO: Include
    }
    
}