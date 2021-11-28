using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Services.Interfaces
{
    public interface IPersonsRepository : IRepositoryBase<Person>
    {
        Task<List<Person>> GetAllPersonListAsync();
    }
}
