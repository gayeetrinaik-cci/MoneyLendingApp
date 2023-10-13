using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Loanconfirmationtoken
{
    public long Id { get; set; }

    public long? LoanId { get; set; }

    public string? ConfirmationCode { get; set; }

    public DateTime? CreationTime { get; set; }

    public DateTime? ExpiryTime { get; set; }

    public virtual Loan? Loan { get; set; }
}
