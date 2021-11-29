using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Services
{
    public class FilmRatingSourcesRepository :  RepositoryBase<FilmRatingSource>, IFilmRatingSourcesRepository
    {
        public FilmRatingSourcesRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}