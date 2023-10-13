using Infrastructure.Entities;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UserRepository : Repository<User>,IUserRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public UserRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var resp = await _dbContext.Users.Include(x => x.Roles).Where(x => x.IsActive).ToListAsync();
            List<UserDTO> users = new List<UserDTO>();

            if (resp != null && resp.Count > 0)
            {
                users.AddRange(resp.Select(user => new UserDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Roles = user.Roles.Select(x => x.Name).ToList()
                }));
            }

            return users;
        }

    }
}
