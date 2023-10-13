using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Company
{
    public class CompanyDetailsDTO : CompanyDTO
    {
        public sbyte? StatusId {  get; set; }
        public DateTime? ApprovedOn { get; set; }
        public long? ApprovedBy { get; set; }
    }
}
