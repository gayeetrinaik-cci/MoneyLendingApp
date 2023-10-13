using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string? ProductType { get; set; }

    public int Duration { get; set; }

    public decimal RateOfInterest { get; set; }

    public int PaymentGap { get; set; }

    public decimal Penalty { get; set; }

    public bool IsPenaltyFixed { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
