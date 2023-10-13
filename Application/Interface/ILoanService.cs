using Infrastructure.Entities;
using Shared.DTO;
using Shared.DTO.Company;
using Shared.DTO.Loan;

namespace Application.Interface
{
    public interface ILoanService
    {
        Task<ResponseDTO<bool>> LoanApplication(LoanDTO loanApplication);
        Task<ResponseDTO<bool>> LoanStatusUpdate(LoanStatusUpdateRequestDTO loanStatusUpdateRequest);
        Task<ResponseDTO<bool>> ConfirmWithLoanToken(string token);
        Task NotifyCompanyOnLoanTerms();
        Task<List<CompanyLoanDTO>> GetCompanyLoans(long companyId);
    }
}
