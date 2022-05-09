using System.ComponentModel.DataAnnotations;

namespace CollegeBackend.Objects.Models;

public class RegisterModel : IUserTargetedModel
{
    [Required] public string FirstName { get; set; }

    [Required] public string SecondName { get; set; }

    [Required] public string Patronymic { get; set; }

    [Required] public string Username { get; set; }

    [Required] public string Password { get; set; }

    [Required] public int PassportId { get; set; }
}