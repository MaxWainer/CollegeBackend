using CollegeBackend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeBackend.Controllers;

[Route("active/[controller]")]
[ApiController]
public class ActiveController : Controller
{
    private readonly CollegeBackendContext _context;

    public ActiveController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    public async Task<ActionResult<Active[]>> ListActives()
    {
        return new ActionResult<Active[]>(
            await _context.Actives.ToArrayAsync());
    }
    
    [HttpPost("remove/{activeId}")]
    [Authorize(Policy = "Administrator")]
    public async Task<JsonResult> RemoveActive(int activeId)
    {
        var active = await _context.Actives.AsQueryable()
            .FirstOrDefaultAsync(active => active.ActiveId == activeId);

        if (active == null)
        {
            return ActiveEnumResult.UnknownActive.ToActionResult();
        }

        await _context.Actives.Remove(active)
            .ReloadAsync();

        return ActiveEnumResult.Success.ToActionResult();
    }

    [HttpPost("add")]
    //[Authorize(Policy = "Administrator")]
    public async Task<JsonResult> AddActive(
        [Bind("StationId", "StartDateTime", "MainDirectionId", "TrainId", "MainStartDateTime")]
        ActiveData activeData)
    {
        // check is train exists
        var trainResult = await _context.Trains.IsExists(train => train.TrainId == activeData.TrainId);

        if (!trainResult)
        {
            return ActiveEnumResult.UnknownTrain.ToActionResult();
        }

        // check is direction exists
        var directionResult =
            await _context.Directions.IsExists(direction => direction.DirectionId == activeData.MainDirectionId);

        if (!directionResult)
        {
            return ActiveEnumResult.UnknownDirection.ToActionResult();
        }

        // check is station exists
        var stationResult =
            await _context.Stations.IsExists(direction => direction.StationId == activeData.StationId);

        if (!stationResult)
        {
            return ActiveEnumResult.UnknownStation.ToActionResult();
        }

        // add async and reload async
        await (await _context.Actives.AddAsync(new Active
            {
                StationId = activeData.StationId, // define station
                StartDateTime = activeData.StartDateTime, // define start date time
                MainStartDateTime = activeData.MainStartDateTime, // define main start date time
                TrainId = activeData.TrainId, // define train id
                MainDirectionId = activeData.MainDirectionId // define main direction
            }))
            .ReloadAsync();

        return ActiveEnumResult.Success.ToActionResult();
    }
}

public enum ActiveEnumResult
{
    UnknownTrain,
    UnknownDirection,
    UnknownStation,
    UnknownActive,
    Success
}

public class ActiveData
{
    public int StationId { get; set; }
    public DateTime StartDateTime { get; set; }
    public int MainDirectionId { get; set; }
    public int TrainId { get; set; }
    public DateTime MainStartDateTime { get; set; }
}