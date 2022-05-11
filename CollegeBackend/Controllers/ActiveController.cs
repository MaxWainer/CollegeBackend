using CollegeBackend.Extensions;
using CollegeBackend.Objects.Database;
using CollegeBackend.Objects.Models;
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
    public async Task<ActionResult<List<Active>>> ListActives()
    {
        return await _context.Actives
            .ToListAsync();
    }

    [HttpPost("remove/{activeId:int}")]
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
        
        await _context.SaveChangesAsync();

        return ActiveEnumResult.Success.ToActionResult();
    }

    [HttpPost("add")]
    [Authorize(Policy = "Administrator")]
    public async Task<JsonResult> AddActive(
        [FromBody] ActiveModule activeModule)
    {
        // check is train exists
        var trainResult = await _context.Trains.IsExists(train => train.TrainId == activeModule.TrainId);

        if (!trainResult)
        {
            return ActiveEnumResult.UnknownTrain.ToActionResult();
        }

        // check is direction exists
        var directionResult =
            await _context.Directions.IsExists(direction => direction.DirectionId == activeModule.MainDirectionId);

        if (!directionResult)
        {
            return ActiveEnumResult.UnknownDirection.ToActionResult();
        }

        // check is station exists
        var stationResult =
            await _context.Stations.IsExists(direction => direction.StationId == activeModule.StationId);

        if (!stationResult)
        {
            return ActiveEnumResult.UnknownStation.ToActionResult();
        }

        // add async and reload async
        await (await _context.Actives.AddAsync(new Active
            {
                StationId = activeModule.StationId, // define station
                StartDateTime = activeModule.StartDateTime, // define start date time
                MainStartDateTime = activeModule.MainStartDateTime, // define main start date time
                TrainId = activeModule.TrainId, // define train id
                MainDirectionId = activeModule.MainDirectionId // define main direction
            }))
            .ReloadAsync();
        
        await _context.SaveChangesAsync();

        return ActiveEnumResult.Success.ToActionResult();
    }
}

public enum ActiveEnumResult
{
    Success,
    UnknownTrain,
    UnknownDirection,
    UnknownStation,
    UnknownActive
}