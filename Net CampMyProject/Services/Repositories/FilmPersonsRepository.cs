using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Services
{
    public class FilmPersonsRepository : RepositoryBase<FilmPerson>, IFilmPersonsRepository
    {
        public FilmPersonsRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}