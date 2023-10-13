using Infrastructure.Entities;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ProductRepository : Repository<Product>,IProductRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public ProductRepository(ApplicationDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<List<Product>> GetProducts(List<long> productIds)
        {
            return await _dbContext.Products.Where(p=> productIds.Contains(p.Id)).ToListAsync();
        }
    }
}
