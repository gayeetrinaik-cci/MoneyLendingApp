using Infrastructure.Entities;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Payment;
using Shared.DTO.Payment.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public PaymentRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<Penalty> GetPenalty(long paymentId)
        {
            return await _dbContext.Penalties.Where(p => p.PaymentId == paymentId).FirstOrDefaultAsync();
        }

        public async Task<List<Payment>> GetPaymentsWithPenalty(List<long> paymentIds)
        {
            return await _dbContext.Payments.Where(p => paymentIds.Contains(p.Id)).Include(p=>p.Penalties).ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentByStatus(int statusId)
        {
            return await _dbContext.Payments.Where(p=>p.StatusId == statusId).ToListAsync();
        }

        public async Task<PaymentDetailsDTO?> GetPaymentById(long paymentId)
        {

            return await _dbContext.Payments.Where(p=>p.Id == paymentId)
                 .Join(
                     _dbContext.Paymentschedules,
                     payment => payment.PaymentScheduleId,
                     paymentSchedule => paymentSchedule.Id,
                    (payment,paymentSchedule) => new
                    {
                        payment,
                        paymentSchedule
                    })
                 .Join(
                _dbContext.Loans,
                paymentDetails => paymentDetails.paymentSchedule.LoanId,
                loan => loan.Id,
                (paymentDetails,loan) => new
                {
                    paymentDetails,
                    loan
                }
                )
                 .Join(
                     _dbContext.Products,
                     paymentLoan => paymentLoan.loan.ProductId,
                     product => product.Id,
                     (paymentLoan, product) => new PaymentDetailsDTO()
                     {
                         LoanId = paymentLoan.loan.Id,
                         ProductId = product.Id,
                         LoanAmount = paymentLoan.loan.Amount,
                         PaymentAmount = paymentLoan.paymentDetails.payment.PaymentAmount,
                         RateOfInterest = product.RateOfInterest,
                         StatusId = paymentLoan.paymentDetails.payment.StatusId??0,
                         Penalty = product.Penalty,
                         IsPenaltyFixed = product.IsPenaltyFixed,
                     }
                ).FirstOrDefaultAsync();

        }

        public async Task<List<Bankaccount>> GetBankDetails(long loanId)
        {
            return await _dbContext.Bankaccounts.Where(b=>b.LoanId == loanId).ToListAsync();
        }

        public async Task<List<Payment>> GetAllPaidPaymentsOfLoan(long loanId)
        {
            var loan = await _dbContext.Loans.Where(x=>x.Id == loanId).Include(x=>x.Paymentschedules).FirstOrDefaultAsync();
            var paymentScheduleIds = loan.Paymentschedules.Select(x=>x.Id).ToList();
            return await _dbContext.Payments.Where(x=> paymentScheduleIds.Contains(x.PaymentScheduleId??0)).Include(x=>x.Penalties).ToListAsync();
        }
    }
}
