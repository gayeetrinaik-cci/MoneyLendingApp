using Shared.DTO.Company.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Company
{
    public class ChangeCompanyStatusDTO
    {
        public long CompanyId { get; set; }
        public CompanyStatus StatusId {  get; set; }
        public long UserId { get; set; }
    }
}
