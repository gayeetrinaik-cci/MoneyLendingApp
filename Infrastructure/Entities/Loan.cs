namespace Infrastructure.Entities;

public partial class Loan
{
    public long Id { get; set; }

    public long CompanyId { get; set; }

    public decimal Amount { get; set; }

    public int ProductId { get; set; }

    public string? LoanReason { get; set; }

    public sbyte StatusId { get; set; }

    public DateTime? AppovedOn { get; set; }

    public long? ApprovedBy { get; set; }

    public DateTime? GrantedOn { get; set; }

    public long? GrantedBy { get; set; }

    public virtual ICollection<Bankaccount> Bankaccounts { get; set; } = new List<Bankaccount>();

    public virtual Company? Company { get; set; }

    public virtual ICollection<Loanconfirmationtoken> Loanconfirmationtokens { get; set; } = new List<Loanconfirmationtoken>();

    public virtual ICollection<Paymentschedule> Paymentschedules { get; set; } = new List<Paymentschedule>();

    public virtual Product? Product { get; set; }

    public virtual Loanstatus? Status { get; set; }
}
