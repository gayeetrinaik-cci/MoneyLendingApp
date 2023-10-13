using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Payment
{
    public class BankDetailsDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
}
