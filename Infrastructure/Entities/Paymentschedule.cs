using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Paymentschedule
{
    public long Id { get; set; }

    public int MonthNumber { get; set; }

    public long LoanId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal Emi { get; set; }

    public decimal CummulativeAmount { get; set; }

    public decimal RemainingPayable { get; set; }

    public virtual Loan? Loan { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
