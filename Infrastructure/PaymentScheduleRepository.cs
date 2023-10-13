using Infrastructure.Entities;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Shared.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class PaymentScheduleRepository : Repository<Paymentschedule>, IPaymentSceduleRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public PaymentScheduleRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

    //    .LeftJoin(
    //            _dbContext.Payments,
    //            paymentSchedule=> paymentSchedule.Id,
    //            payment=> payment
    //            (paymentSchedule, payment) => new
    //            {
    //                paymentSchedule,
    //                payment
    //}
    //            )

        public async Task<List<GeneratePaymentDTO>> GetPaymentSchedulesLiableForPayment()
        {
            return await _dbContext.Paymentschedules.Where(ps => ps.PaymentDate <= DateTime.Today).Include(p=>p.Payments).Where(p=>p.Payments == null || p.Payments.Count == 0)
                .Join(
                     _dbContext.Loans,
                     paymentSchedule => paymentSchedule.LoanId,
                     loan => loan.Id,
                    (paymentSchedule, loan) => new
                    {
                        paymentSchedule,
                        loan
                    })
                 .Join(
                     _dbContext.Products,
                     paymentLoan => paymentLoan.loan.ProductId,
                     product => product.Id,
                     (paymentLoan, product) => new GeneratePaymentDTO()
                     {
                         PaymentSchedule = new Shared.DTO.PaymentSchedule.PaymentScheduleDTO()
                         {
                             CummulativeAmount = paymentLoan.paymentSchedule.CummulativeAmount,
                             Emi = paymentLoan.paymentSchedule.Emi,
                             LoanAmount = paymentLoan.loan.Amount,
                             LoanId = paymentLoan.paymentSchedule.LoanId,
                             MonthNumber = paymentLoan.paymentSchedule.MonthNumber,
                             //PayableAmount = paymentLoan.paymentSchedule.PayableAmount,
                             PaymentDate = paymentLoan.paymentSchedule.PaymentDate,
                             Id = paymentLoan.paymentSchedule.Id,
                             RateOfInterest = product.RateOfInterest,
                             RemainingPayable = paymentLoan.paymentSchedule.RemainingPayable
                         },
                         Product = new Shared.DTO.Product.ProductDTO()
                         {
                             Duration = product.Duration,
                             RateOfInterest = product.RateOfInterest
                         }

                     }
                ).ToListAsync();
        }

        public async Task<Paymentschedule> GetPaymentscheduleByMonthAndLoan(int monthNumber, long loanId)
        {
            return await _dbContext.Paymentschedules.Where(ps => ps.LoanId == loanId && ps.MonthNumber == monthNumber).Include(ps=>ps.Payments).FirstOrDefaultAsync();
        }
    }
}
