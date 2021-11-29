using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Services
{
    public class FilmRatingsRepository : RepositoryBase<FilmRating>, IFilmRatingsRepository
    {
        public FilmRatingsRepository(ApplicationDbContext db) : base(db)
        {
        }

        public override Task<FilmRating> GetByIdAsync(int id)
        {
            return GetAll().Include(c => c.Source).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}