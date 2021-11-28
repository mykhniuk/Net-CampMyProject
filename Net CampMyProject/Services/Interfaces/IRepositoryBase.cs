using System;
using System.Linq;
using System.Threading.Tasks;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Services.Interfaces
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : DbEntityBase<int>
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetByIdAsync(int id);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task<bool> ExistsAsync(int id);
        Task DeleteByIdAsync(int id);
    }
}