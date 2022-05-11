namespace CollegeBackend.Objects.Database;

public class Ticket
{
    public int TicketId { get; set; }
    public int RelatedDirectionId { get; set; }
    public int RelatedActiveId { get; set; }

    public DateTime StartDate { get; set; }
    public int PassportId { get; set; }
    public int EndStationId { get; set; }
    public int SittingId { get; set; }

    public virtual Station EndStation { get; set; } = null!;
    public virtual User? Passport { get; set; }
    public virtual Active RelatedActive { get; set; } = null!;
    public virtual Direction RelatedDirection { get; set; } = null!;
    public virtual Sitting Sitting { get; set; } = null!;
}