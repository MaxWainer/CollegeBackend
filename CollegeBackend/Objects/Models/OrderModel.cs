using System.ComponentModel.DataAnnotations;

namespace CollegeBackend.Objects.Models;

public class OrderModel
{
    [Required] public int TrainId { get; set; }
    [Required] public int CarriageId { get; set; }
    [Required] public int ActiveId { get; set; }
    [Required] public int PassportId { get; set; }
    [Required] public int SittingId { get; set; }
    [Required] public int EndStationId { get; set; }
}