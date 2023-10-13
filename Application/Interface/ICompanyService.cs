using Shared.DTO;
using Shared.DTO.Company;

namespace Application.Interface
{
    public interface ICompanyService
    {
        Task<List<CompanyDetailsDTO>> GetAll();

        Task<ResponseDTO<long>> RegisterCompany(CompanyDTO company);

        Task NotifyCompanyRegistration();

        Task<ResponseDTO<bool>> UpdateCompanyStatus(ChangeCompanyStatusDTO changeCompanyStatusRequest);

        Task<CompanyPaymentDetailsDTO> GetCompanyPaymentDetails(long companyId);
    }
}
