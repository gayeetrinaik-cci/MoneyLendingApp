using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Companystatus
{
    public sbyte Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
}
