using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.PaymentSchedule
{
    public class PaymentScheduleDTO
    {
        public long Id { get; set; }

        public int MonthNumber { get; set; }

        public long LoanId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal LoanAmount { get; set; }

        public decimal RateOfInterest { get; set; }

        public decimal PayableAmount { get; set; }

        public decimal Emi { get; set; }

        public decimal CummulativeAmount { get; set; }

        public decimal RemainingPayable { get; set; }

        public decimal PrincipleAmount { get; set; }

        public decimal Interest { get; set; }
    }
}
