using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Payment
{
    public class PaymentDTO
    {
        public long Id { get; set; }

        public long? PaymentScheduleId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal PaymentAmount { get; set; }

        public decimal CummulativePaid { get; set; }

        public decimal RemainingPayment { get; set; }

        public decimal Principle {  get; set; }

        public decimal Interest {  get; set; }

        public int? StatusId { get; set; }

        public decimal Penalty { get; set; }
    }
}
