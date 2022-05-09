using System;
using System.Collections.Generic;

namespace CollegeBackend.Objects.Database;

public partial class Sitting
{
    public Sitting()
    {
        Tickets = new HashSet<Ticket>();
    }

    public int SitId { get; set; }
    public string Index { get; set; } = null!;
    public int RelatedCarriageId { get; set; }

    public virtual Carriage RelatedCarriage { get; set; } = null!;
    public virtual ICollection<Ticket> Tickets { get; set; }
}