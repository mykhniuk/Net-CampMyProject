using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Services
{
    public class FilmGenresRepository : RepositoryBase<FilmGenre>, IFilmGenresRepository
    {
        public FilmGenresRepository(ApplicationDbContext db) : base(db)
        {
        }

        public override IQueryable<FilmGenre> GetAll()
        {
            return base.GetAll().Include(c=>c.Film);
        }

        public override async Task<FilmGenre> GetByIdAsync(int id)
        {
            return await GetAll()
                .Include(f => f.Film)
                .FirstOrDefaultAsync(m => m.Id == id); ;
        }
    }
}