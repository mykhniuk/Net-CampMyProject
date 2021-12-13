using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Services
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : DbEntityBase<int>
    {
        private readonly ApplicationDbContext _db;

        protected RepositoryBase(ApplicationDbContext db)
        {
            _db = db;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _db.Set<TEntity>().AsNoTracking().AsQueryable();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            _db.Add(entity);
            await _db.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _db.Update(entity);
            await _db.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _db.Set<TEntity>().AsNoTracking().AnyAsync(e => e.Id == id);
        }

        public virtual async Task DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _db.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}