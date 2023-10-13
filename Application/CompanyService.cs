using Application.Interface;
using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.Interface;
using Infrastructure.Services.EmailService.Interface;
using Shared.DTO;
using Shared.DTO.Company;
using Shared.DTO.Company.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application
{
    public class CompanyService : ICompanyService
    {
        #region Private 
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILoanService _loanService;
        #endregion Private 

        #region Constructor
        public CompanyService(ICompanyRepository repository, IUserRepository userRepository, IEmailService emailService,
            ILoanService loanService)
        {
            _companyRepository = repository;
            _userRepository = userRepository;
            _emailService = emailService;
            _loanService = loanService;

        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// This will be called from backgroung job to send email to all Admin users regarding new company registrations
        /// </summary>
        /// <returns></returns>
        public async Task NotifyCompanyRegistration()
        {
            var companies = await _companyRepository.GetAllUnProcessed();

            if (companies != null && companies.Count > 0)
            {
                var users = await _userRepository.GetAllUsers();
                if (users != null && users.Count > 0)
                {
                    var admins = users.Where(x => x.Roles.Contains("Admin"));
                    if (admins != null && admins.Count() > 0)
                    {
                        foreach (var company in companies)
                        {
                            foreach (var admin in admins.Select(x => x.Email))
                            {
                                await _emailService.SendEmailAsync(new List<string>() { admin }, $"{company.Name} - Registered", $"{company.Name} registered with us. Kindly check the profile and update.");
                            }
                            company.StatusId = (int)CompanyStatus.AdminNotified;
                            await _companyRepository.Update(company, company.Id);
                        }
                    }
                }
            }
        }
        
        public async Task<ResponseDTO<long>> RegisterCompany(CompanyDTO company)
        {
            ResponseDTO<long> response = new ResponseDTO<long>();
            var res = await ValidateCompanyRegistrationRequest(company);
            if(res != null && res.Error != null)
            {
                return res;
            }

            var createCompanyResponse = await _companyRepository.Create(new Company()
            {
                Name = company.Name,
                Email = company.Email,
                RegistrationDate = company.RegistrationDate,
                RegistrationNumber = company.RegistrationNumber,
                Description = company.Description,
                StatusId = (int)CompanyStatus.NotProcessed
            });

            if(createCompanyResponse != null)
            {
                response.Data = createCompanyResponse.Id;
            }

            return response;
        }

        public async Task<ResponseDTO<bool>> UpdateCompanyStatus(ChangeCompanyStatusDTO changeCompanyStatusRequest)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();
            var res = await ValidateUpdateCompanyStatus(changeCompanyStatusRequest);
            if (res != null && res.Error != null)
            {
                return res;
            }

            response.Data = await _companyRepository.UpdateCompanyStatus(changeCompanyStatusRequest);

            return response;
        }

        public async Task<List<CompanyDetailsDTO>> GetAll()
        {
            return await _companyRepository.GetAllCompanies();
        }

        public async Task<CompanyPaymentDetailsDTO> GetCompanyPaymentDetails(long companyId)
        {
            CompanyPaymentDetailsDTO companyDetails = new CompanyPaymentDetailsDTO();
            var company = await _companyRepository.GetById(companyId);

            if(company != null)
            {
                companyDetails.Id = company.Id;
                companyDetails.Name = company.Name ?? string.Empty;
                companyDetails.RegistrationNumber = company.RegistrationNumber??string.Empty;
                companyDetails.Loans = await _loanService.GetCompanyLoans(companyId);
            }

            return companyDetails;
        }

        #endregion public methods

        #region Private Methods
        private async Task<ResponseDTO<long>> ValidateCompanyRegistrationRequest(CompanyDTO company)
        {
            ResponseDTO<long> response = new ResponseDTO<long>();
            var companyResponse = await _companyRepository.FindByName(company.Name);
            if (companyResponse != null) 
            {
                response.Error = new ResponseError()
                {
                    ErrorCode = (int)HttpStatusCode.BadRequest,
                    Message = "Company is already registered"
                };
            }
            return response;
        }

        private async Task<ResponseDTO<bool>> ValidateUpdateCompanyStatus(ChangeCompanyStatusDTO changeCompanyStatusRequest)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();
            var company = await _companyRepository.GetById(changeCompanyStatusRequest.CompanyId);
            if(company==null)
            {
                response.Error = new ResponseError()
                {
                    ErrorCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid Company"
                };
            }

            return response;
        }
        #endregion Private Methods
    }
}
