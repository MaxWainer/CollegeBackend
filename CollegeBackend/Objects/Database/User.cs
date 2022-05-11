namespace CollegeBackend.Objects.Database;

public class User
{
    public User()
    {
        Tickets = new HashSet<Ticket>();
    }

    public int PassportId { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public string Role { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; }
}