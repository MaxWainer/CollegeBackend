using System;
using System.Collections.Generic;

namespace CollegeBackend
{
    public partial class User
    {
        public User()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int PassportId { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string Patronymic { get; set; } = null!;

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
