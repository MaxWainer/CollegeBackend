using System.ComponentModel.DataAnnotations;

namespace CollegeBackend.Objects.Models;

public class ActiveModule
{
    [Required] public int StationId { get; set; }
    [Required] public DateTime StartDateTime { get; set; }
    [Required] public int MainDirectionId { get; set; }
    [Required] public int TrainId { get; set; }
    [Required] public DateTime MainStartDateTime { get; set; }
}