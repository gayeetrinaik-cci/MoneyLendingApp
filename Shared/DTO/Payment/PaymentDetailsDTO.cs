using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Payment
{
    public class PaymentDetailsDTO
    {
        public long LoanId { get; set; }
        public long ProductId { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal RateOfInterest { get; set; }
        public decimal LoanAmount { get; set; }
        public bool IsPenaltyFixed { get; set; }
        public decimal Penalty { get; set; }
        public int StatusId { get; set; }
    }
}
