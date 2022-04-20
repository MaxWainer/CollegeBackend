using System;
using System.Collections.Generic;

namespace CollegeBackend
{
    public partial class Direction
    {
        public Direction()
        {
            Actives = new HashSet<Active>();
            Stations = new HashSet<Station>();
            Tickets = new HashSet<Ticket>();
        }

        public int DirectionId { get; set; }
        public string Name { get; set; } = null!;
        public int StartStationId { get; set; }
        public int EndStationId { get; set; }

        public virtual ICollection<Active> Actives { get; set; }
        public virtual ICollection<Station> Stations { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
