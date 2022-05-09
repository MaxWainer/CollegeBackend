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
        public int Role { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        public bool HasRole(int roles)
        {
            return (Role & roles) == roles;
        }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
