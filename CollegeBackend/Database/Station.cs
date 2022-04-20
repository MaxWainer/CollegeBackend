using System;
using System.Collections.Generic;

namespace CollegeBackend
{
    public partial class Station
    {
        public Station()
        {
            Actives = new HashSet<Active>();
            Tickets = new HashSet<Ticket>();
        }

        public int StationId { get; set; }
        public int RelatedDirection { get; set; }
        public string Name { get; set; } = null!;

        public virtual Direction RelatedDirectionNavigation { get; set; } = null!;
        public virtual ICollection<Active> Actives { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
