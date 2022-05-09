using System.ComponentModel.DataAnnotations;

namespace CollegeBackend.Objects.Models;

public class LoginModel : IUserTargetedModel
{
    [Required] public string Username { get; set; }

    [Required] public string Password { get; set; }

    [Required] public int PassportId { get; set; }
}