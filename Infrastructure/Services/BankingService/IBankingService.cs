using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.BankingService
{
    public interface IBankingService
    {
        Task<decimal> GetBankBalance(string bankName, string accountNumber);
        Task<bool> MakePayment(PaymentRequest paymentRequest);
    }
}
