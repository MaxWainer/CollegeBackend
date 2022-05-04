using CollegeBackend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("order/[controller]")]
[Authorize(Policy = "User")]
public class OrderTicketDataController : Controller
{
    private readonly CollegeBackendContext _context;

    public OrderTicketDataController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Create(
        [Bind("PassportId", "SittingId", "ActiveId", "TrainId", "CarriageId")]
        OrderData orderData)
    {
        // under review
        // ----------------
        // var activeQuery =
        //     from active in _context.Actives
        //     where active.TrainId == orderData.TrainId &&
        //           active.Train.Carriages.ContainsPredicate(
        //               carriage => carriage.CarriageId == orderData.CarriageId)
        //     select active;

        // define query where we select all tickets
        // with queried active id and sitting id
        var sitQuery =
            from ticket in _context.Tickets
            where ticket.RelatedActiveId == orderData.ActiveId
                  && ticket.SittingId == orderData.SittingId
            select ticket;

        // get first ticket where passport is null
        var possibleTicket = await sitQuery.FirstOrDefaultAsync(ticket => ticket.Passport == null);

        // if it's null, ticket already created or not exists
        if (possibleTicket == null)
        {
            return OrderTicketEnumResult.AlreadyCreatedOrNotExists.ToActionResult();
        }

        // get user
        var user = await _context.Users.FindAsync(orderData.PassportId);

        // if null, smth goes extremely wrong
        if (user == null)
        {
            return OrderTicketEnumResult.UserNotExists.ToActionResult();
        }

        // set to ticket new passport
        possibleTicket.Passport = user;
        
        // and passport id
        possibleTicket.PassportId = orderData.PassportId;

        // update it
        await _context.Tickets.Update(possibleTicket).ReloadAsync();

        // success
        return OrderTicketEnumResult.Success.ToActionResult();
    }
}

public enum OrderTicketEnumResult
{
    Success,
    AlreadyCreatedOrNotExists,
    UserNotExists
}

public class OrderData
{
    public int TrainId { get; set; }
    public int CarriageId { get; set; }
    public int ActiveId { get; set; }
    public int PassportId { get; set; }
    public int SittingId { get; set; }
}