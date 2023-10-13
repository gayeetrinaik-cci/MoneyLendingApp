using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Loan
{
    public class LoanDTO
    {
        [Key]
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public int ProductId { get; set; }
        public decimal LoanAmount { get; set; }
        public string LoanReason { get; set; }

        public List<BankAccountDTO> BankAccounts { get; set; } = new List<BankAccountDTO>();

    }
}
