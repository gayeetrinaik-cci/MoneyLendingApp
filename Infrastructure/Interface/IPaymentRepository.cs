using Infrastructure.Entities;
using Shared.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Penalty> GetPenalty(long paymentId);
        Task<List<Payment>> GetPaymentsWithPenalty(List<long> paymentIds);
        Task<List<Payment>> GetPaymentByStatus(int statusId);
        Task<PaymentDetailsDTO> GetPaymentById(long paymentId);
        Task<List<Bankaccount>> GetBankDetails(long loanId);
        Task<List<Payment>> GetAllPaidPaymentsOfLoan(long loanId);


    }
}
