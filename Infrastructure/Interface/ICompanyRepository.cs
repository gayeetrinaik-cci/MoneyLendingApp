using Infrastructure.Entities;
using Shared.DTO;
using Shared.DTO.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<List<Company>> GetAllUnProcessed();
        Task<bool> UpdateCompanyStatus(ChangeCompanyStatusDTO changeCompanyStatusRequest);
        Task<List<CompanyDetailsDTO>> GetAllCompanies();

        Task<Company> FindByName(string name);
    }
}
