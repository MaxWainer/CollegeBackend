using CollegeBackend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeBackend.Controllers;

[Microsoft.AspNetCore.Components.Route("addActive/[controller]")]
[Authorize(Policy = "Administrator")]
public class AddActiveController : Controller
{
    private readonly CollegeBackendContext _context;

    public AddActiveController(CollegeBackendContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<string>> AddActive(
        [Bind("StationId", "StartDateTime", "MainDirectionId", "TrainId", "MainStartDateTime")]
        ActiveData activeData)
    {
        // check is train exists
        var trainResult = await _context.Trains.IsExists(train => train.TrainId == activeData.TrainId);

        if (!trainResult)
        {
            return AddActiveEnumResult.UnknownTrain.ToActionResult();
        }

        // check is direction exists
        var directionResult =
            await _context.Directions.IsExists(direction => direction.DirectionId == activeData.MainDirectionId);

        if (!directionResult)
        {
            return AddActiveEnumResult.UnknownDirection.ToActionResult();
        }

        // check is station exists
        var stationResult =
            await _context.Stations.IsExists(direction => direction.StationId == activeData.StationId);

        if (!stationResult)
        {
            return AddActiveEnumResult.UnknownStation.ToActionResult();
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

        return AddActiveEnumResult.Success.ToActionResult();
    }
}

public enum AddActiveEnumResult
{
    UnknownTrain,
    UnknownDirection,
    UnknownStation,
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