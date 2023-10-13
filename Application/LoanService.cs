using Application.Interface;
using Infrastructure.Entities;
using Infrastructure.Interface;
using Infrastructure.Services.EmailService.Interface;
using Shared.DTO;
using Shared.DTO.Company;
using Shared.DTO.Company.Enums;
using Shared.DTO.Loan;
using Shared.DTO.Loan.Enum;
using Shared.DTO.PaymentSchedule;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Application
{
    public class LoanService : ILoanService
    {
        #region private
        private readonly ILoanRepository _loanRepository;
        private readonly IEmailService _emailService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        #endregion private

        #region constructor
        public LoanService(ILoanRepository loanRepository, IEmailService emailService, ICompanyRepository companyRepository, IUserRepository userRepository)
        {
            _loanRepository = loanRepository;
            _emailService = emailService;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
        }
        #endregion constructor

        #region public methds
        public async Task<ResponseDTO<bool>> LoanApplication(LoanDTO loanApplication)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();
            var validationResponse = await ValidateLoanApplicationRequest(loanApplication);
            if(validationResponse != null && validationResponse.Error != null)
            {
                return validationResponse;
            }

            await _loanRepository.Create(new Infrastructure.Entities.Loan()
            {
                CompanyId = loanApplication.CompanyId,
                Amount = loanApplication.LoanAmount,
                LoanReason = loanApplication.LoanReason,
                ProductId = loanApplication.ProductId,
                StatusId = (int)LoanStatus.NotProcessed,
                Bankaccounts = loanApplication.BankAccounts.Select(x=>new Infrastructure.Entities.Bankaccount()
                {
                    BankName = x.BankName,
                    AccountNumber = x.AccountNumber
                }).ToList()
            });
            response.Data = true;
            return response;
        }

        public async Task<ResponseDTO<bool>> LoanStatusUpdate(LoanStatusUpdateRequestDTO loanStatusUpdateRequest)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();

            var validationResponse = await ValidateUpdateStatusRequest(loanStatusUpdateRequest);
            if (validationResponse != null && validationResponse.Error != null)
            {
                return validationResponse;
            }

            var loan = await _loanRepository.GetById(loanStatusUpdateRequest.LoanId);
            loan.StatusId = loanStatusUpdateRequest.LoanStatusId;
            if (loanStatusUpdateRequest.LoanStatusId == (int)LoanStatus.Granted)
            {
                loan.GrantedBy = loanStatusUpdateRequest.UserId;
                loan.GrantedOn = DateTime.UtcNow;
            }
            else
            {
                loan.ApprovedBy = loanStatusUpdateRequest.UserId;
                loan.AppovedOn = DateTime.UtcNow;
            }

            await _loanRepository.Update(loan, loan.Id);

            response.Data = true;
            return response;
        }

        public async Task NotifyCompanyOnLoanTerms()
        {
            //Get all the loans in approved status
            //create loan terms data and send to company via email with confirmation code
            var approvedLoans = await _loanRepository.GetLoansWithApprovedStatus();

            foreach (var loan in approvedLoans)
            {
                var confirmationCode = Guid.NewGuid().ToString();
                var lonTerms = GenerateLoanTermsMessage(loan, confirmationCode);
                await _emailService.SendEmailAsync(new List<string>() { loan.Company.Email }, "Terms Of Loan", lonTerms);

                loan.StatusId = (int)LoanStatus.ApprovalNotified;
                await _loanRepository.Update(loan, loan.Id);

                await _loanRepository.CreateLoanconfirmationToken(new Loanconfirmationtoken()
                {
                    ConfirmationCode = confirmationCode,
                    LoanId = loan.Id,
                    CreationTime = DateTime.UtcNow,
                    ExpiryTime = DateTime.UtcNow.AddDays(1)
                });
            }
        }

        public async Task<ResponseDTO<bool>> ConfirmWithLoanToken(string token)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();
            var tokenInfo = await _loanRepository.GetLoanConfirmationToken(token);
            if(tokenInfo == null || tokenInfo.ExpiryTime < DateTime.UtcNow)
            {
                response.Error = new ResponseError()
                {
                    ErrorCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid or expired token"
                };

                return response;
            }

            if(tokenInfo != null && tokenInfo.ExpiryTime > DateTime.UtcNow)
            {
                var loan = await _loanRepository.GetById(tokenInfo.LoanId??0);
                loan.StatusId = (int)LoanStatus.TermsAccepted;
                await _loanRepository.Update(loan, loan.Id);
            }

            response.Data = true;
            return response;
        }


        public async Task<List<CompanyLoanDTO>> GetCompanyLoans(long companyId)
        {
            List<CompanyLoanDTO> loanDetails = new List<CompanyLoanDTO>();
            var loans = await _loanRepository.GetCompanyLoanDetails(companyId);
            if (loans != null && loans.Count > 0)
            {
                foreach (var loan in loans)
                {
                    loanDetails.Add(new CompanyLoanDTO()
                    {
                        Id = loan.Id,
                        LoanAmount = loan.Amount,
                        BankDetails = loan.Bankaccounts.Select(x=> new Shared.DTO.Payment.BankDetailsDTO()
                        {
                            Id=x.Id,
                            AccountNumber = x.AccountNumber??string.Empty,
                            Balance = x.Balance,
                            Name = x.BankName??string.Empty
                        }).ToList(),
                        PaymentSchedule = loan.Paymentschedules.Select(x=> new PaymentScheduleDTO()
                        {
                            Id = x.Id,
                            CummulativeAmount = x.CummulativeAmount,
                            Emi = x.Emi,// Math.Round(loan.Amount * (loan.Product.RateOfInterest / loan.Product.Duration), 2),
                            Interest = Math.Round(((Math.Round(loan.Amount * loan.Product.RateOfInterest, 2)) - loan.Amount) / loan.Product.Duration, 2),
                            RateOfInterest = loan.Product.RateOfInterest,
                            LoanAmount = loan.Amount,
                            LoanId = loan.Id,
                            MonthNumber = x.MonthNumber,
                            PayableAmount = Math.Round(loan.Amount * loan.Product.RateOfInterest, 2),//x.PayableAmount,
                            PaymentDate = x.PaymentDate,
                            PrincipleAmount = Math.Round((Math.Round(loan.Amount * (loan.Product.RateOfInterest / loan.Product.Duration), 2)) / loan.Product.RateOfInterest, 2),
                            RemainingPayable = x.RemainingPayable
                        }).ToList(),
                        Product = new Shared.DTO.Product.ProductDTO()
                        {
                            Id = loan.Product.Id,
                            Duration = loan.Product.Duration,
                            IsPenaltyFixed = loan.Product.IsPenaltyFixed,
                            Penalty = loan.Product.Penalty,
                            RateOfInterest = loan.Product.RateOfInterest
                        }
                    });

                    var payments = await _loanRepository.GetAllPaymentsOfLoan(loan.Id);
                    if(payments != null && payments.Count > 0)
                    {
                        loanDetails.Last().Payments = payments.Select(x=>new Shared.DTO.Payment.PaymentDTO()
                        {
                            CummulativePaid = x.CummulativePaid,
                            Id = x.Id,
                            PaymentDate = x.PaymentDate,
                            PaymentScheduleId = x.PaymentScheduleId,
                            PaymentAmount = x.PaymentAmount,
                            RemainingPayment = x.RemainingPayment,
                            Principle = Math.Round(x.PaymentAmount/ (loanDetails.Last().Product.RateOfInterest),2),
                            Interest = Math.Round(x.PaymentAmount - (x.PaymentAmount / (loanDetails.Last().Product.RateOfInterest)),2),
                            Penalty = Math.Round(x.Penalties.Sum(x=>x.Amount),2),
                            StatusId = x.StatusId
                        }).ToList();
                    }
                }
            }

            return loanDetails;
        }

        #endregion public methods

        #region private methods
        private string GenerateLoanTermsMessage(Loan loan, string confirmationCode)
        {
            StringBuilder LoanTerms = new StringBuilder();
            LoanTerms.Append("Months: ");
            LoanTerms.AppendLine( Convert.ToString(loan.Product.Duration));
            LoanTerms.Append("Penalty : ");
            LoanTerms.Append(loan.Product.Penalty);
            if (!(loan.Product.IsPenaltyFixed))
            {
                LoanTerms.Append("x");
            }
            LoanTerms.AppendLine("");
            LoanTerms.Append("Confirmation Code : ");
            LoanTerms.Append(confirmationCode);

            return LoanTerms.ToString();
        }

        private async Task<ResponseDTO<bool>> ValidateLoanApplicationRequest(LoanDTO loan)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();
            if (loan == null)
            {
                response.Error = new ResponseError() { ErrorCode = (int)HttpStatusCode.BadRequest, Message = "Invalid Request" };
                return response;
            }
            if (loan.LoanAmount <= 0)
            {
                response.Error = new ResponseError() { ErrorCode = (int)HttpStatusCode.BadRequest, Message = "Invalid loan amount" };
                return response;
            }
            if (loan.BankAccounts == null || loan.BankAccounts.Count == 0)
            {
                response.Error = new ResponseError() { ErrorCode = (int)HttpStatusCode.BadRequest, Message = "Please provide bank details" };
                return response;
            }
            //TODO
            //validate companyid
            var company = await _companyRepository.GetById(loan.CompanyId);
            if(company == null || company.StatusId != (int)CompanyStatus.Approved)
            {
                response.Error = new ResponseError() { ErrorCode = (int)HttpStatusCode.BadRequest, Message = "Company not registered or authorized for loan" };
                return response;
            }
            //validate product id
            return response;
        }

        public async Task<ResponseDTO<bool>> ValidateUpdateStatusRequest(LoanStatusUpdateRequestDTO loanStatusUpdateRequest)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();
            if (loanStatusUpdateRequest == null)
            {
                response.Error = new ResponseError() { ErrorCode = (int)HttpStatusCode.BadRequest, Message = "Invalid Request" };
                return response;
            }
            if(loanStatusUpdateRequest.LoanId == 0 || await _loanRepository.GetById(loanStatusUpdateRequest.LoanId) == null)
            {
                response.Error = new ResponseError() { ErrorCode = (int)HttpStatusCode.NotFound, Message = "Loan not found." };
                return response;
            }
            if(loanStatusUpdateRequest.UserId == 0 || await _userRepository.GetById(loanStatusUpdateRequest.UserId) == null)
            {
                response.Error = new ResponseError() { ErrorCode = (int)HttpStatusCode.BadRequest, Message = "Invalid User" };
                return response;
            }

            return response;
        }
        #endregion private methods

    }
}
