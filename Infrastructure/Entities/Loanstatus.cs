using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Loanstatus
{
    public sbyte Id { get; set; }

    public int? StatusId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
