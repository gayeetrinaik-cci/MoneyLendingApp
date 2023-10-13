using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(object id);
        Task<T> Create(T entity);
        Task<T> Update(T entity, long id);
        Task<bool> DeleteById(long id);
        Task<List<T>> CreateMany(List<T> entities);

    }
}
