using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Penalty
{
    public long Id { get; set; }

    public long? PaymentId { get; set; }

    public decimal Amount { get; set; }

    public virtual Payment? Payment { get; set; }
}
