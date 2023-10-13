using System;
using System.Collections.Generic;

namespace Infrastructure.Entities;

public partial class Bankaccount
{
    public long Id { get; set; }

    public long? LoanId { get; set; }

    public string? BankName { get; set; }

    public string? AccountNumber { get; set; }

    public decimal Balance { get; set; }

    public virtual Loan? Loan { get; set; }
}
