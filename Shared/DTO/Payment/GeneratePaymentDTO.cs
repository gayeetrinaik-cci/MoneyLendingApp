using Shared.DTO.PaymentSchedule;
using Shared.DTO.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Payment
{
    public class GeneratePaymentDTO 
    {
        public PaymentScheduleDTO PaymentSchedule { get; set; }

        public ProductDTO Product { get; set; }
    }
}
