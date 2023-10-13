using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Private members
        private readonly ApplicationDBContext _dbContext;
        private readonly DbSet<T> _entity;
        #endregion Private members

        #region Constructor
        public Repository(ApplicationDBContext dBContext) 
        { 
            _dbContext = dBContext;
            _entity = dBContext.Set<T>();
        }
        #endregion Constructor

        public async Task<T> Create(T entity)
        {
            await _entity.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<T>> CreateMany(List<T> entities)
        {
            await _entity.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        public Task<bool> DeleteById(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetAll()
        {
            return await _entity.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<T> Update(T entity, long id)
        {
            var entityToUpdate = await _entity.FindAsync(id);
            entityToUpdate = entity;
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
