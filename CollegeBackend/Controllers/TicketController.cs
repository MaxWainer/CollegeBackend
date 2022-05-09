using CollegeBackend.Extensions;
using CollegeBackend.Objects.Database;
using CollegeBackend.Objects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("ticket/[controller]")]
[ApiController]
public class TicketController : Controller
{
    private readonly CollegeBackendContext _context;

    public TicketController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpPost("list/{passportId}")]
    public async Task<ActionResult<List<Ticket>>> ListTickets(int passportId)
    {
        // just select by passport id
        var where =
            from ticket in _context.Tickets
            where ticket.PassportId == passportId
            select ticket;

        return (await where.ToListAsync()).ToActionResult();
    }

    [HttpPost("order/{orderModel}")]
    [Authorize(Policy = "User")]
    public async Task<JsonResult> OrderTicket(
        [Bind("PassportId", "SittingId", "ActiveId", "TrainId", "CarriageId")]
        OrderModel orderModel)
    {
        // define query where we select all tickets
        // with queried active id and sitting id
        var sitQuery =
            from ticket in _context.Tickets
            where ticket.RelatedActiveId == orderModel.ActiveId
                  && ticket.SittingId == orderModel.SittingId
                  && ticket.Sitting.RelatedCarriageId == orderModel.CarriageId
                  && ticket.Sitting.RelatedCarriage.RelatedTrainId == orderModel.TrainId
            select ticket;

        // get first ticket where passport is null
        var possibleTicket = await sitQuery.FirstOrDefaultAsync(ticket => ticket.Passport == null);

        // if it's null, ticket already created or not exists
        if (possibleTicket == null)
        {
            return OrderTicketEnumResult.AlreadyCreatedOrNotExists.ToActionResult();
        }

        // get user
        var user = await _context.Users.FindAsync(orderModel.PassportId);

        // if null, smth goes extremely wrong
        if (user == null)
        {
            return OrderTicketEnumResult.UserNotExists.ToActionResult();
        }

        // set to ticket new passport
        possibleTicket.Passport = user;

        // and passport id
        possibleTicket.PassportId = orderModel.PassportId;

        // update it
        await _context.Tickets.Update(possibleTicket).ReloadAsync();

        // success
        return OrderTicketEnumResult.Success.ToActionResult();
    }

    [HttpGet("delete/{ticketId}")]
    [Authorize(Policy = "Administrator")]
    public async Task<JsonResult> DeleteTicket(int ticketId)
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

public enum OrderTicketEnumResult
{
    Success, // 0
    AlreadyCreatedOrNotExists, // 1
    UserNotExists // 2
}

public enum DeleteTicketEnumResult
{
    Success,
    TicketNotFound
}