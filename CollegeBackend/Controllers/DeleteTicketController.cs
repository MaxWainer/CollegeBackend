using CollegeBackend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeBackend.Controllers;

[Route("delete/[controller]")]
[Authorize(Policy = "Administrator")]
public class DeleteTicketController : Controller
{
    private readonly CollegeBackendContext _context;

    public DeleteTicketController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteTicket(int ticketId)
    {
        // we use it twice
        var tickets = _context.Tickets;

        // select all tickets with specific id
        var result = (from ticket in tickets
                where ticket.TicketId == ticketId
                select ticket
            ).FirstOrDefault();

        // if result is null, return ticket not found
        if (result == null)
        {
            return DeleteTicketEnumResult.TicketNotFound.ToActionResult();
        }

        // else we remove and reload it
        await tickets.Remove(result)
            .ReloadAsync();

        // success
        return DeleteTicketEnumResult.Success.ToActionResult();
    }
}

public enum DeleteTicketEnumResult
{
    Success,
    TicketNotFound
}