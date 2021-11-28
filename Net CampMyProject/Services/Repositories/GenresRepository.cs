using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Services
{
    public class GenresRepository : RepositoryBase<Genre>, IGenresRepository
    {
        public GenresRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}