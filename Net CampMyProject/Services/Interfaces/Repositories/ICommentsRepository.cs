using System.Collections.Generic;
using Net_CampMyProject.Data.Models;
using System.Threading.Tasks;

namespace Net_CampMyProject.Services.Interfaces
{
    public interface ICommentsRepository : IRepositoryBase<Comment>
    {
        Task<List<Comment>> GetAllCommentsListAsync();
    }
}