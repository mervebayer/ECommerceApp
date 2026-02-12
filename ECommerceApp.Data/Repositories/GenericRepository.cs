using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(AppDbContext context) {
            _context = context;
            _dbSet = _context.Set<T>();

        }
        public async Task<T> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);      
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;

                var entry = _context.Entry(entity);
                if (entry.State == EntityState.Detached)
                    _dbSet.Attach(entity);

                _context.Entry(entity).Property(nameof(BaseEntity.IsDeleted)).IsModified = true;
                return;
            }

            _dbSet.Remove(entity);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);         
        }
    }
}
