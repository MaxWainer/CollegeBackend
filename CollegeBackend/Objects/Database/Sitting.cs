namespace CollegeBackend.Objects.Database;

public class Sitting
{
    public int SitId { get; set; }
    public string Index { get; set; } = null!;
    public int RelatedCarriageId { get; set; }
    
    public string SitType { get; set; }
    
    public int Price { get; set; }

    public virtual Carriage RelatedCarriage { get; set; } = null!;
    public virtual Ticket? Ticket { get; set; }
}