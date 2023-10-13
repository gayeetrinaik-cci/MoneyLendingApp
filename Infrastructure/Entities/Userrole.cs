using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public partial class Userrole
{
    [Key]
    public long UserId { get; set; }

    [Key]
    public long RoleId { get; set; }

    //public virtual ICollection<User> Users { get; set; } = new List<User>();
    //public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
