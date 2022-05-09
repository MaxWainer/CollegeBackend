using CollegeBackend.Objects.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("sittings/[controller]")]
[ApiController]
public class SittingController : Controller
{
    private readonly CollegeBackendContext _context;

    public SittingController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpPost("list")]
    public async Task<ActionResult<List<Sitting>>> ListSittings()
    {
        return await _context.Sittings.ToListAsync();
    }
    
}