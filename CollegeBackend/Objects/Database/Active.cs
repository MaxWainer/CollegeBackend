namespace CollegeBackend.Objects.Database;

public class Active
{
    public Active()
    {
        Tickets = new HashSet<Ticket>();
    }

    public int ActiveId { get; set; }
    public int StationId { get; set; }

    public DateTime StartDateTime { get; set; }
    public int MainDirectionId { get; set; }
    public int TrainId { get; set; }

    public DateTime MainStartDateTime { get; set; }

    public virtual Direction MainDirection { get; set; } = null!;
    public virtual Station Station { get; set; } = null!;
    public virtual ICollection<Train> Trains { get; set; } = null!;
    public virtual ICollection<Ticket> Tickets { get; set; }
}