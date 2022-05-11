using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CollegeBackend.Objects.Models;

public class ClearCacheModel
{
    [Required] public string Token { get; set; }
}