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

    [HttpPost("list/{passportId:int}")]
    [Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<ActionResult<List<Ticket>>> ListTickets(int passportId)
    {
        // just select by passport id
        var where =
            from ticket in _context.Tickets
            where ticket.PassportId == passportId
            select ticket; // TODO: Include

        return await where.ToListAsync();
    }

    [HttpPost("order")]
    //[Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<JsonResult> OrderTicket(
        [FromBody]
        OrderModel orderModel)
    {
        // define query where we select all tickets
        // with queried active id and sitting id
        var sitQuery =
            from sitting in _context.Sittings
            where sitting.RelatedCarriage.RelatedTrain.Active.ActiveId == orderModel.ActiveId
                  && sitting.SitId == orderModel.SittingId
                  && sitting.RelatedCarriageId == orderModel.CarriageId
                  && sitting.RelatedCarriage.RelatedTrainId == orderModel.TrainId
            select sitting;

        // get first ticket where passport is null
        var possibleSitting = await sitQuery.FirstOrDefaultAsync(sitting => sitting.Ticket == null);

        // if it's null, ticket already created or not exists
        if (possibleSitting == null) return OrderTicketEnumResult.AlreadyCreatedOrNotExists.ToActionResult();

        // check end station
        if (!await _context.Stations.IsExists(station => station.StationId == orderModel.EndStationId))
            return OrderTicketEnumResult.InvalidEndStation.ToActionResult();

        // get user
        var user = await _context.Users.FindAsync(orderModel.PassportId);

        // if null, smth goes extremely wrong
        if (user == null) return OrderTicketEnumResult.UserNotExists.ToActionResult();
        
        // get active
        var active = await _context.Actives.FirstOrDefaultAsync(active => active.ActiveId == orderModel.ActiveId);

        // check active
        if (active == null) return OrderTicketEnumResult.InvalidActive.ToActionResult();

        // get train
        var train = await _context.Trains.FirstOrDefaultAsync(train => train.TrainId == orderModel.TrainId);
            
        // check train
        if (train == null) return OrderTicketEnumResult.InvalidTrain.ToActionResult();

        // create new ticket
        var ticket = new Ticket
        {
            RelatedActiveId = orderModel.ActiveId,
            RelatedDirectionId = active.MainDirectionId,
            StartDate = active.StartDateTime,
            PassportId = orderModel.PassportId,
            SittingId = orderModel.SittingId,
            EndStationId = orderModel.EndStationId
        };

        // set ticket to sitting
        possibleSitting.Ticket = ticket;

        // update sitting
        await _context.Sittings.Update(possibleSitting).ReloadAsync();

        // add new ticket
        await _context.Tickets.AddAsync(ticket);
        
        await _context.SaveChangesAsync();

        // success
        return OrderTicketEnumResult.Success.ToActionResult();
    }

    [HttpGet("delete/{ticketId:int}")]
    [Authorize(Roles = "Administrator,Moderator")]
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
        
        await _context.SaveChangesAsync();

        // success
        return DeleteTicketEnumResult.Success.ToActionResult();
    }
}

public enum OrderTicketEnumResult
{
    Success, // 0
    AlreadyCreatedOrNotExists, // 1
    UserNotExists, // 2
    InvalidActive, // 3
    InvalidTrain, // 4
    InvalidEndStation // 5
}

public enum DeleteTicketEnumResult
{
    Success,
    TicketNotFound
}