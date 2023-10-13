using Infrastructure.Entities;
using Shared.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<UserDTO>> GetAllUsers();
    }
}
