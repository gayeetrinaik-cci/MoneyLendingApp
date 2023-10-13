using Infrastructure.Entities;
using Shared.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IPaymentSceduleRepository : IRepository<Paymentschedule>
    {
        Task<List<GeneratePaymentDTO>> GetPaymentSchedulesLiableForPayment();

        Task<Paymentschedule> GetPaymentscheduleByMonthAndLoan(int monthNumber, long loanId);
    }
}
