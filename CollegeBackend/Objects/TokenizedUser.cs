using CollegeBackend.Objects.Database;

namespace CollegeBackend.Objects;

public class TokenizedUser
{
    public User User { get; set; }

    public Guid Token { get; set; }
}