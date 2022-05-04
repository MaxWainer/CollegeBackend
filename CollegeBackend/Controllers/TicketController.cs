using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

[assembly: ApiController]

namespace CollegeBackend.Controllers;

[Route("ticket/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ILogger<TicketsController> _logger;
    private readonly CollegeBackendContext _context;

    public TicketsController(ILogger<TicketsController> logger, CollegeBackendContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerator<Ticket>> Get(int passportId)
    {
        var where = _context.Tickets.Where(ticket => ticket.PassportId == passportId);
        return new ActionResult<IEnumerator<Ticket>>(where.GetEnumerator());
    }
}