using AuthServer.Core.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext? _context;
        private readonly DbSet<TEntity>? _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet= context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity =await _dbSet.FindAsync(id);
            if (entity!=null)
            {
                //lets entity not be added to memory
                _context.Entry(entity).State= EntityState.Detached;
            }
            return entity;
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public TEntity Update(TEntity entity)
        {
            //one of generic repository's disadvantage is, the code below will
            //make a product's entire column's be shown as updated, so it slows down the process.
            //When used savechanges, it only updates changed column.
            //for example: product.name=='samsung'. This only updates name.
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}