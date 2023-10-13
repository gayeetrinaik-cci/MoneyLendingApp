using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Company
{
    public class CompanyPaymentDetailsDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }  
        public string RegistrationNumber { get; set; }
        public List<CompanyLoanDTO> Loans {get;set;}
    }
}
