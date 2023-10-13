using Shared.DTO.Loan;
using Shared.DTO.Payment;
using Shared.DTO.PaymentSchedule;
using Shared.DTO.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Company
{
    public class CompanyLoanDTO
    {
        public long Id { get; set; }

        public decimal LoanAmount { get; set; }

        public ProductDTO Product { get; set; }

        public List<BankDetailsDTO> BankDetails { get; set; }

        public List<PaymentScheduleDTO> PaymentSchedule { get; set; }

        public List<PaymentDTO> Payments { get; set; }
    }
}
