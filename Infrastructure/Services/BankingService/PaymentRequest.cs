using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.BankingService
{
    public class PaymentRequest
    {
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public DateTime date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
}
