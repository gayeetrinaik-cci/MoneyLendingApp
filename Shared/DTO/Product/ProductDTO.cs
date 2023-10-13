using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Product
{
    public class ProductDTO
    {
        public long Id {  get; set; }
        public int Duration {  get; set; }
        public decimal RateOfInterest { get; set; }
        public decimal Penalty { get; set; }
        public bool IsPenaltyFixed { get; set; }
    }
}
