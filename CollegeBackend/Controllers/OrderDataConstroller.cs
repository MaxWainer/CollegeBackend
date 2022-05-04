using CollegeBackend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;

namespace CollegeBackend.Controllers;

[Route("order/[controller]")]
public class OrderDataController : Controller
{
    private readonly ILogger<OrderDataController> _logger;
    private readonly CollegeBackendContext _context;

    public OrderDataController(ILogger<OrderDataController> logger, CollegeBackendContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Create(
        [Bind("PassportId", "SittingId", "ActiveId", "TrainId", "CarriageId")]
        OrderData orderData)
    {
        var active = _context.Actives
            .Where(active => active.TrainId == orderData.TrainId)
            .Where(active =>
                active.Train.Carriages.ContainsPredicate(carriage => carriage.CarriageId == orderData.CarriageId));

        var sitQuery = _context.Tickets
            .Where(ticket => ticket.RelatedActiveId == orderData.ActiveId)
            .Where(ticket => ticket.SittingId == orderData.SittingId);

        var possibleTicket = await sitQuery.FirstOrDefaultAsync(ticket => ticket.Passport == null);

        if (possibleTicket != null)
        {
            return new ActionResult<string>(EnumResult.AlreadyCreated.ToString());
        }

        var ticket = await sitQuery.FirstOrDefaultAsync();

        if (ticket == null)
        {
            return new ActionResult<string>(EnumResult.SittingNotExists.ToString());
        }

        var user = await _context.Users.FindAsync(orderData.PassportId);

        if (user == null)
        {
            return new ActionResult<string>(EnumResult.UserNotExists.ToString());
        }

        ticket.Passport = user;
        ticket.PassportId = orderData.PassportId;

        await _context.Tickets.Update(ticket).ReloadAsync();

        return new ActionResult<string>(EnumResult.Success.ToString());
    }
}

public enum EnumResult
{
    Success,
    AlreadyCreated,
    SittingNotExists,
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