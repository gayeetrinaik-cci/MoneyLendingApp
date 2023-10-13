using Infrastructure.Entities;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Company;
using Shared.DTO.Company.Enums;
using System.Collections.Generic;

namespace Infrastructure
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDBContext _dBContext;
        public CompanyRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<List<Company>> GetAllUnProcessed()
        {
            return await _dBContext.Companies.Where(x=>x.StatusId == (int)CompanyStatus.NotProcessed).ToListAsync();
        }

        public async Task<bool> UpdateCompanyStatus(ChangeCompanyStatusDTO changeCompanyStatusRequest)
        {
            var company = await this.GetById(changeCompanyStatusRequest.CompanyId);
            company.StatusId = (sbyte)changeCompanyStatusRequest.StatusId;
            company.ApprovedOn = DateTime.UtcNow;
            company.ApprovedBy = changeCompanyStatusRequest.UserId;
            await this.Update(company,company.Id);
            return true;
        }

        public async Task<List<CompanyDetailsDTO>> GetAllCompanies()
        {
            var companies = await this.GetAll();
            List<CompanyDetailsDTO> companiesResponse = new List<CompanyDetailsDTO>();
            if (companies!= null && companies.Count > 0)
            {
                companiesResponse.AddRange(companies.Select(company=>new CompanyDetailsDTO()
                {
                    Id = company.Id,
                    Name = company.Name,
                    Email = company.Email,
                    Description = company.Description,
                    RegistrationDate = company.RegistrationDate,
                    RegistrationNumber = company.RegistrationNumber,
                    StatusId = company.StatusId,
                    ApprovedBy = company.ApprovedBy,
                    ApprovedOn = company.ApprovedOn
                }));
            }

            return companiesResponse;
        }

        public async Task<Company> FindByName(string name)
        {
            return await _dBContext.Companies.Where(x => x.Name.Equals(name)).FirstOrDefaultAsync();
        }

        //public async Task<CompanyPaymentDetailsDTO> GetCompanyPaymentDetails(long companyId)
        //{
        //    CompanyPaymentDetailsDTO details = new CompanyPaymentDetailsDTO();
        //    var companyLoans = await _dBContext.Companies.Where(x => x.Id == companyId).Include(x => x.Loans).FirstOrDefaultAsync();
        //    details.Id = companyLoans.Id;
        //    details.Name = companyLoans.Name;
        //    details.RegistrationNumber = companyLoans.RegistrationNumber;
        //    details.Loans = new List<CompanyLoanDTO>();
        //    details.Loans.AddRange(companyLoans.Loans.Select(x => new CompanyLoanDTO()
        //    {
        //        Id = x.Id,
        //        LoanAmount = x.Amount,
        //    }).ToList());

        //    foreach(var loan in details.Loans)
        //    {
        //        loan.BankDetails = await 
        //    }
        //}
    }
}
