using Application.Interface;
using Infrastructure.Entities;
using Infrastructure.Interface;
using Infrastructure.Services;
using Infrastructure.Services.BankingService;
using Shared.DTO;
using Shared.DTO.Payment.Enum;
using System.Net;

namespace Application
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentSceduleRepository _paymentSceduleRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IHttpClientAgent _httpClientAgent;
        private readonly IBankingService _bankingService;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IPenaltyRepository _penaltyRepository;

        public PaymentService(IPaymentSceduleRepository paymentSceduleRepository, IPaymentRepository paymentRepository, ILoanRepository loanRepository, 
            IHttpClientAgent httpClientAgent, IBankingService bankingService, IProductRepository productRepository
            ,IBankAccountRepository bankAccountRepository, IPenaltyRepository penaltyRepository)
        {
            _paymentSceduleRepository = paymentSceduleRepository;
            _paymentRepository = paymentRepository;
            _loanRepository = loanRepository;
            _httpClientAgent = httpClientAgent;
            _bankingService = bankingService;
            _bankAccountRepository = bankAccountRepository;
            _penaltyRepository = penaltyRepository;
        }

        public async Task GeneratePayment()
        {
            //Get payment schedules for which payment needs to be raised i.e payment date <= today and payment is null
            var paymentSchedules = await _paymentSceduleRepository.GetPaymentSchedulesLiableForPayment();
            
            foreach (var paymentSchedule in paymentSchedules)
            {
                var payment = new Payment()
                {
                    PaymentAmount = paymentSchedule.PaymentSchedule.Emi,
                    PaymentScheduleId = paymentSchedule.PaymentSchedule.Id,
                    StatusId = (int)PaymentStatus.Pending,
                };

                //Check for penalty in previous payment and add the same to payment amount
                if (paymentSchedule.PaymentSchedule.MonthNumber > 1)
                {
                    var previousMonthsPaymentSchdule = await _paymentSceduleRepository.GetPaymentscheduleByMonthAndLoan(paymentSchedule.PaymentSchedule.MonthNumber-1, paymentSchedule.PaymentSchedule.LoanId);
                    
                    if(previousMonthsPaymentSchdule.Payments != null && previousMonthsPaymentSchdule.Payments.Count > 0)
                    {
                        if (previousMonthsPaymentSchdule.Payments.FirstOrDefault().StatusId == (int)PaymentStatus.Failed)
                        {
                            var penalty = await _paymentRepository.GetPenalty(previousMonthsPaymentSchdule.Payments.FirstOrDefault().Id);
                            if (penalty != null)
                            {
                                payment.PaymentAmount = payment.PaymentAmount + penalty.Amount;
                            };
                            payment.PaymentAmount = payment.PaymentAmount + previousMonthsPaymentSchdule.Payments.FirstOrDefault().PaymentAmount;
                        }
                        else
                        {
                            payment.RemainingPayment = previousMonthsPaymentSchdule.Payments.FirstOrDefault().RemainingPayment;
                        }
                    }
                }
                else if(paymentSchedule.PaymentSchedule.MonthNumber == 1)
                {
                    payment.RemainingPayment = Math.Round(paymentSchedule.PaymentSchedule.RemainingPayable + payment.PaymentAmount,2);
                }

                await _paymentRepository.Create(payment);
                #region commented
                ////Adjust the payment in case of last payment
                //if (paymentSchedule.Product.Duration == paymentSchedule.PaymentSchedule.MonthNumber)
                //{
                //    //(Get all penalties + total payable amount with interest) == (Total payment amount)
                //    var allPaymentSchedulesForLoan = await _loanRepository.GetLoanWithPayments(paymentSchedule.PaymentSchedule.LoanId);
                //    if (allPaymentSchedulesForLoan != null && allPaymentSchedulesForLoan.Paymentschedules != null && allPaymentSchedulesForLoan.Paymentschedules.Count > 0)
                //    {
                //        List<long> paymentScheduleIds = allPaymentSchedulesForLoan.Paymentschedules.Select(ps => ps.Id).ToList();
                //        var penaltyDetails = await _paymentRepository.GetPaymentsWithPenalty(paymentScheduleIds);
                //        var penalties = penaltyDetails.SelectMany(p => p.Penalties);
                //        var totalPenalty = penalties.Sum(p=>p.Amount);
                //        var totalPayableAmountWithInterest = Math.Round(paymentSchedule.PaymentSchedule.LoanAmount * paymentSchedule.Product.RateOfInterest,2);
                //    }
                //}
                #endregion commented
            }
            
        }

        public async Task<ResponseDTO<bool>> MakePayment(long paymentId)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>();
            var paymentDetails = await _paymentRepository.GetPaymentById(paymentId);

            if(paymentDetails == null)
            {
                response.Error = new ResponseError()
                {
                    ErrorCode = (int)HttpStatusCode.NotFound,
                    Message = "Payment record not found"
                };

                return response;
            }
            else if (paymentDetails.StatusId != (int)PaymentStatus.Pending)
            {
                response.Error = new ResponseError()
                {
                    ErrorCode = (int)HttpStatusCode.BadRequest,
                    Message = "The requested payment is not in pending payment status"
                };

                return response;
            }
            else
            {
                var bankDetails = await _paymentRepository.GetBankDetails(paymentDetails.LoanId);
                var allPayments = await _paymentRepository.GetAllPaidPaymentsOfLoan(paymentDetails.LoanId);

                decimal taotalPaymentWithInterest = Math.Round(paymentDetails.LoanAmount * paymentDetails.RateOfInterest, 2);
                var paidPayments = allPayments.Where(x => x.StatusId == (int)PaymentStatus.Paid);
                var records = allPayments.Where(x => x.Id != paymentId);
                Payment previousPayment = null;
                if (records != null && records.Count() > 0)
                {
                    previousPayment = records.OrderBy(x => x.Id).LastOrDefault();
                }
                decimal cummulativePaid = 0;
                if (paidPayments != null && paidPayments.Count() > 0)
                {
                    cummulativePaid = Math.Round(paidPayments.Sum(x => x.PaymentAmount), 2);
                }
                var penalties = allPayments.SelectMany(x => x.Penalties);
                decimal totalPenalty = 0;
                if (penalties != null)
                {
                    totalPenalty = penalties.Sum(x => x.Amount);
                }

                foreach (var bank in bankDetails)
                {
                    var balance = await _bankingService.GetBankBalance(bank.BankName, bank.AccountNumber);
                    bank.Balance = balance;
                }
                var maxBalance = bankDetails.Max(x => x.Balance);
                var maxBalanceAccount = bankDetails.Where(x => x.Balance == maxBalance).FirstOrDefault();
                var payment = await _paymentRepository.GetById(paymentId);
                //payment.RemainingPayment = Math.Round(taotalPaymentWithInterest + totalPenalty - cummulativePaid, 2);

                bool paid = false;
                if (maxBalanceAccount != null && maxBalanceAccount.Balance >= paymentDetails.PaymentAmount)
                {
                    //create transaction with the bank 
                    paid = await _bankingService.MakePayment(new PaymentRequest()
                    {
                        AccountNumber = maxBalanceAccount.AccountNumber ?? string.Empty,
                        BankName = maxBalanceAccount.BankName ?? string.Empty,
                        Amount = paymentDetails.PaymentAmount,
                        date = DateTime.UtcNow,
                        Type = "debit"
                    });
                }

                if(paid)
                {
                    payment.StatusId = (int)PaymentStatus.Paid;
                    payment.CummulativePaid = cummulativePaid + paymentDetails.PaymentAmount;
                    //payment.RemainingPayment = Math.Round(taotalPaymentWithInterest + totalPenalty - (cummulativePaid+ paymentDetails.PaymentAmount), 2);
                    //payment.RemainingPayment = previousPayment == null || previousPayment.Id == 0 ? payment.RemainingPayment: Math.Round(previousPayment.RemainingPayment+ payment.PaymentAmount, 2);
                    payment.RemainingPayment = previousPayment == null || previousPayment.Id == 0 ? payment.RemainingPayment - +payment.PaymentAmount : Math.Round(previousPayment.RemainingPayment - payment.PaymentAmount, 2);
                    payment.PaymentDate = DateTime.UtcNow;
                    await _paymentRepository.Update(payment, paymentId);
                    maxBalanceAccount.Balance = maxBalanceAccount.Balance - paymentDetails.PaymentAmount;
                    await UpateBankOutstandingAmount(bankDetails);
                }
                else
                {
                    //decimal remainingPayment = Math.Round(taotalPaymentWithInterest + totalPenalty - cummulativePaid, 2);
                    decimal remainingPayment = previousPayment == null ? payment.RemainingPayment : previousPayment.RemainingPayment;
                    //add penalty
                    decimal penalty = Math.Round(paymentDetails.IsPenaltyFixed ? paymentDetails.Penalty : remainingPayment * paymentDetails.Penalty, 2);
                    await _penaltyRepository.Create(new Penalty() { PaymentId = paymentId, Amount = penalty });
                    payment.StatusId = (int)PaymentStatus.Failed;
                    payment.CummulativePaid = cummulativePaid;
                    payment.RemainingPayment = Math.Round(remainingPayment + penalty,2);
                    await _paymentRepository.Update(payment, paymentId);
                    //set payment status to failed
                    response.Error = new ResponseError()
                    {
                        ErrorCode = 0,
                        Message = "Insufficient Balance"
                    };
                }

            }

            response.Data = true;
            return response;
        }

        private async Task UpateBankOutstandingAmount(List<Bankaccount> bankDetails)
        {
            foreach (var bank in bankDetails)
            {
                await _bankAccountRepository.Update(bank, bank.Id);
            }
        }
    }
}
