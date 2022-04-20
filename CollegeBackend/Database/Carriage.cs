using System;
using System.Collections.Generic;

namespace CollegeBackend
{
    public partial class Carriage
    {
        public Carriage()
        {
            Sittings = new HashSet<Sitting>();
        }

        public int CarriageId { get; set; }
        public string Index { get; set; } = null!;
        public int RelatedTrainId { get; set; }

        public virtual Train RelatedTrain { get; set; } = null!;
        public virtual ICollection<Sitting> Sittings { get; set; }
    }
}
