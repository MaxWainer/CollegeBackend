namespace CollegeBackend.Objects.Models;

public interface IUserTargetedModel
{
    string Username { get; set; }
    
    string Password { get; set; }
    
    int PassportId { get; set; }
}