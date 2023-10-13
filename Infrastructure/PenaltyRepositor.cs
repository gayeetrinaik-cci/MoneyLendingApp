using Infrastructure.Entities;
using Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class PenaltyRepositor : Repository<Penalty>, IPenaltyRepository
    {
        public PenaltyRepositor(ApplicationDBContext dBContext) : base(dBContext)
        {
        }
    }
}
