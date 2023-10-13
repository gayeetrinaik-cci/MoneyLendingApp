using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Payment
{
    public long Id { get; set; }

    public long? PaymentScheduleId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal PaymentAmount { get; set; }

    public decimal CummulativePaid { get; set; }

    public decimal RemainingPayment { get; set; }

    public int? StatusId { get; set; }

    public virtual Paymentschedule? PaymentSchedule { get; set; }

    public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
}
