using Infrastructure.Entities;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Loan.Enum;
using Shared.DTO.Payment.Enum;

namespace Infrastructure
{
    public class LoanRepository: Repository<Loan>, ILoanRepository
    {
        private readonly ApplicationDBContext _dBContext;

        public LoanRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<List<Loan>> GetLoansWithApprovedStatus()
        {
            return await _dBContext.Loans.Where(loan=>loan.StatusId==(int)LoanStatus.Approved).Include(x=>x.Product).Include(x=>x.Company).ToListAsync();
        }

        public async Task<bool> CreateLoanconfirmationToken(Loanconfirmationtoken loanConfirmationToken)
        {
            try
            {
                await _dBContext.Loanconfirmationtokens.AddAsync(loanConfirmationToken);
                await _dBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Loanconfirmationtoken> GetLoanConfirmationToken(string loanConfirmationToken)
        {
            return await _dBContext.Loanconfirmationtokens.Where(x=>x.ConfirmationCode.Equals(loanConfirmationToken)).FirstOrDefaultAsync();
        }

        public async Task<List<Loan>> GetLoanByStatus(int statusId)
        {
            return await _dBContext.Loans.Where(loan=>loan.StatusId == statusId).ToListAsync();
        }

        public async Task<Loan> GetLoanWithPayments(long loanId)
        {
            return await _dBContext.Loans.Where(loan=>loan.Id == loanId).Include(loan=>loan.Paymentschedules).FirstOrDefaultAsync();
        }

        public async Task<List<Loan>> GetCompanyLoanDetails(long companyId)
        {
            return await _dBContext.Loans.Where(x=>x.CompanyId == companyId).Include(x=>x.Bankaccounts).Include(x=>x.Paymentschedules).Include(x=>x.Product).ToListAsync();
        }

        public async Task<List<Payment>> GetAllPaymentsOfLoan(long loanId)
        {
            var loan = await _dBContext.Loans.Where(x => x.Id == loanId).Include(x => x.Paymentschedules).FirstOrDefaultAsync();
            var paymentScheduleIds = loan.Paymentschedules.Select(x => x.Id).ToList();
            return await _dBContext.Payments.Where(x => paymentScheduleIds.Contains(x.PaymentScheduleId ?? 0)).Include(x => x.Penalties).ToListAsync();
        }
    }
}
