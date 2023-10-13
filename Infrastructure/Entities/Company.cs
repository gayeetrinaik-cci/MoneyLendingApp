using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Company
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? RegistrationNumber { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public string? Description { get; set; }

    public sbyte? StatusId { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public long? ApprovedBy { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual Companystatus? Status { get; set; }
}
