using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;

[assembly: ApiController]

namespace CollegeBackend.Controllers;

[Route("ticket/[controller]")]
[Authorize(Policy = "User")]
public class TicketArrayController : ControllerBase
{
    private readonly CollegeBackendContext _context;

    public TicketArrayController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<List<Ticket>>> Get(int passportId)
    {
        // just select by passport id
        var where =
            from ticket in _context.Tickets
            where ticket.PassportId == passportId
            select ticket;
        
        return new ActionResult<List<Ticket>>(await where.ToListAsync());
    }
}