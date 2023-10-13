using Application.Interface;
using Infrastructure.Entities;
using Infrastructure.Interface;
using Shared.DTO.Loan.Enum;
using Shared.DTO.PaymentSchedule;
using System.Collections.Generic;

namespace Application
{
    public class PaymentScheduleService : IPaymentScheduleService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaymentSceduleRepository _paymentSceduleRepository;

        public PaymentScheduleService(IPaymentSceduleRepository paymentSceduleRepository,ILoanRepository loanRepository, IProductRepository productRepository)
        {
            _loanRepository = loanRepository;
            _productRepository = productRepository;
            _paymentSceduleRepository = paymentSceduleRepository;
        }
        public async Task GeneratePaymentSchedule()
        {
           //get loan
           var loans = await _loanRepository.GetLoanByStatus((int)LoanStatus.Granted);
            foreach (var loan in loans)
            {
                var product = await _productRepository.GetById(loan.ProductId);
                decimal EMI = Math.Round(loan.Amount * (product.RateOfInterest / product.Duration),2);
                decimal totalToPay = 0;
                decimal totalPayableWithInterest = Math.Round(loan.Amount * product.RateOfInterest,2);
                decimal monthlyInterest = Math.Round((totalPayableWithInterest - loan.Amount) / product.Duration,2);
                DateTime paymentDate = loan.GrantedOn.Value;

                List<PaymentScheduleDTO> paymentSchedule = new List<PaymentScheduleDTO>();
                for (int i = 1; i <= product.Duration; i++)
                {
                    int gap = i == 1 ? 0 : product.PaymentGap;
                    totalToPay += EMI;
                    paymentDate = new DateTime(paymentDate.AddMonths(1+ gap).Year, paymentDate.AddMonths(1 + gap).Month, 1);
                    paymentSchedule.Add(new PaymentScheduleDTO()
                    {
                        MonthNumber = i,
                        PaymentDate = paymentDate,
                        LoanAmount = loan.Amount,
                        RateOfInterest = product.RateOfInterest,
                        PayableAmount = totalPayableWithInterest,
                        Emi = EMI,
                        CummulativeAmount = totalToPay,
                        RemainingPayable = Math.Round(totalPayableWithInterest - totalToPay,2),
                        Interest = monthlyInterest,
                        PrincipleAmount = Math.Round(EMI / product.RateOfInterest,2),// EMI - monthlyInterest,
                        LoanId = loan.Id
                    }) ;
                }

                var adjustedPaymentSchedule = AdjustAndReturnPaymentSchedule(paymentSchedule, product.Duration, monthlyInterest);
                await _paymentSceduleRepository.CreateMany(adjustedPaymentSchedule);
                loan.StatusId = (int)LoanStatus.PaymentScheduleGenerated;
                await _loanRepository.Update(loan, loan.Id);
            }
        }

        private List<Paymentschedule> AdjustAndReturnPaymentSchedule(List<PaymentScheduleDTO> paymentSchedule, int duration, decimal monthlyInterest)
        {
            List<Paymentschedule> schedule = new List<Paymentschedule>();
            if (paymentSchedule != null && paymentSchedule.Count > 1)
            {
                decimal totalPayableWithInterest = paymentSchedule[0].PayableAmount;
                decimal totalCummulative = paymentSchedule.Last().CummulativeAmount;
                if (totalCummulative != totalPayableWithInterest)
                {
                    decimal difference = Math.Abs(totalPayableWithInterest - totalCummulative);
                    decimal lastEMI = 0;

                    if (totalPayableWithInterest > totalCummulative)
                    {
                        lastEMI = paymentSchedule[duration - 1].Emi + difference;

                    }
                    else if (totalPayableWithInterest < totalCummulative)
                    {
                        lastEMI = paymentSchedule[duration - 1].Emi - difference;
                    }

                    paymentSchedule[duration - 1].Emi = lastEMI;
                    paymentSchedule[duration - 1].CummulativeAmount = paymentSchedule[duration - 2].CummulativeAmount + lastEMI; ;
                    paymentSchedule[duration - 1].PrincipleAmount = lastEMI - monthlyInterest;
                    paymentSchedule[duration - 1].RemainingPayable = paymentSchedule[duration - 2].RemainingPayable - paymentSchedule[duration - 1].Emi;//Math.Round(totalPayableWithInterest - totalToPay, 2),
                }
            }

            if (paymentSchedule != null && paymentSchedule.Count > 0)
            {
                schedule.AddRange(paymentSchedule.Select(x=>new Paymentschedule()
                {
                    Id = x.Id,
                    CummulativeAmount=x.CummulativeAmount,
                    Emi=x.Emi,
                    LoanId=x.LoanId,
                    MonthNumber=x.MonthNumber,
                    PaymentDate=x.PaymentDate,
                    RemainingPayable=x.RemainingPayable,
                }));
            }

            return schedule;
        }
    }
}
