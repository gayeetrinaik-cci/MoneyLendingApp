using Infrastructure.Entities;
using Shared.DTO;
using Shared.DTO.Loan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface ILoanRepository : IRepository<Loan>
    {
        Task<List<Loan>> GetLoansWithApprovedStatus();
        Task<bool> CreateLoanconfirmationToken(Loanconfirmationtoken loanConfirmationToken);
        Task<Loanconfirmationtoken> GetLoanConfirmationToken(string loanConfirmationToken);
        Task<List<Loan>> GetLoanByStatus(int statusId);
        Task<Loan> GetLoanWithPayments(long loanId);
        Task<List<Loan>> GetCompanyLoanDetails(long companyId);
        Task<List<Payment>> GetAllPaymentsOfLoan(long loanId);
    }
}
