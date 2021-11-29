using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Services
{
    public class MyRatingsRepository : RepositoryBase<MyFilmRating>, IMyRatingsRepository
    {
        public MyRatingsRepository(ApplicationDbContext db) : base(db)
        {
        }

        public override IQueryable<MyFilmRating> GetAll()
        {
            return base.GetAll().Include(c=>c.Author).Include(c=>c.Film);
        }

        public override async Task<MyFilmRating> GetByIdAsync(int id)
        {
            return await GetAll().Include(m => m.Author)
                .Include(m => m.Film)
                .FirstOrDefaultAsync(m => m.Id == id); ;
        }

        public async Task<MyFilmRating> CreateOrUpdateAsync(MyFilmRating myFilmRating)
        {
            return await GetAll().FirstOrDefaultAsync(mr =>
                mr.FilmId == myFilmRating.FilmId && mr.AuthorId == myFilmRating.AuthorId);
        }
    }
}