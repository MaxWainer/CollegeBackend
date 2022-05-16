using CollegeBackend.Auth;
using CollegeBackend.Extensions;
using CollegeBackend.Objects.Database;
using CollegeBackend.Objects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("ticket/[controller]")]
[ApiController]
public sealed class TicketController : Controller
{
    private readonly CollegeBackendContext _context;

    public TicketController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpGet("list/{passportId:int}")]
    [Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<ActionResult<List<Ticket>>> ListTickets(int passportId)
    {
        // just select by passport id
        var where = _context.Tickets
            .Where(ticket => ticket.PassportId == passportId)
            .Include(ticket => ticket.EndStation)
            .Include(ticket => ticket.RelatedActive)
            .Include(ticket => ticket.RelatedDirection)
            .Include(ticket => ticket.Sitting)
            .ThenInclude(sitting => sitting.RelatedCarriage)
            .ThenInclude(carriage => carriage.RelatedTrain);

        return await where.ToListAsync();
    }
    
    [HttpPost("order")]
    [Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<JsonResult> OrderTicket(
        [FromBody] OrderModel orderModel)
    {
        // define query where we select all tickets
        // with queried active id and sitting id
        var sitQuery =
            from s in _context.Sittings
            join a in _context.Actives on s.RelatedCarriage.RelatedTrainId equals a.TrainId
            where a.ActiveId == orderModel.ActiveId
                  && s.SitId == orderModel.SittingId
                  && s.RelatedCarriageId == orderModel.CarriageId
                  && s.RelatedCarriage.RelatedTrainId == orderModel.TrainId
                  && s.Ticket == null
            select s;

        // get first ticket where passport is null
        var possibleSitting = await sitQuery.FirstOrDefaultAsync();

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
    
    [HttpGet("multiDelete")]
    [Authorize(Roles = "User,Administrator,Moderator")]
    public async Task<JsonResult> MultiDelete([FromBody] MultiTicketDeleteModel deleteModel)
    {
        
        // we use it twice
        var tickets = _context.Tickets;

        // select all tickets with specific id
        var result = await (from ticket in tickets
                where deleteModel.TicketIds.Contains(ticket.TicketId)
                select ticket
            ).ToListAsync();

        // if result is null, return ticket not found
        if (result.Count <= 0)
        {
            return DeleteTicketEnumResult.TicketNotFound.ToActionResult();
        }

        // else we remove and reload it
        tickets.RemoveRange(result);

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