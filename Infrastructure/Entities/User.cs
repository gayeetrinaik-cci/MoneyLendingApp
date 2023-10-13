using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public partial class User
    {
        public long Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
