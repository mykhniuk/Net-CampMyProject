using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Controllers;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;
using Net_CampMyProject.Services.Results;

namespace Net_CampMyProject.Services
{
    public class FilmsRepository : IFilmsRepository, IDisposable
    {
        private readonly ApplicationDbContext _db;

        public FilmsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<FilteredFilmsResult> GetFilteredAsync(FilmsFilterType filterType, string sortBy, SortOrder sortOrder, int page, int pageSize, string authorId)
        {
            var filmsQueryBase = _db.Films
                .AsNoTracking().AsSplitQuery()
                .Include(c => c.MyRatings)
                .Include(c => c.Genres)
                .ThenInclude(c => c.Genre)
                .Include(c => c.Ratings)
                .ThenInclude(c => c.Source);

            var filmsQuery = filterType switch
            {
                FilmsFilterType.All => filmsQueryBase,
                FilmsFilterType.Liked => filmsQueryBase.Where(f => f.MyRatings.Any(r => r.AuthorId == authorId && r.MyRating == true)),
                _ => throw new ArgumentOutOfRangeException(nameof(filterType), filterType, "Unknown 'filterType'")
            };

            if (sortOrder == SortOrder.Unspecified)
                sortOrder = SortOrder.Descending;

            var isDesc = sortOrder == SortOrder.Descending;

            filmsQuery = sortBy switch
            {
                nameof(Film.Title) => isDesc ? filmsQuery.OrderByDescending(s => s.Title) : filmsQuery.OrderBy(s => s.Title),
                nameof(Film.ReleaseDate) => isDesc
                    ? filmsQuery.OrderByDescending(s => s.ReleaseDate)
                    : filmsQuery.OrderBy(s => s.ReleaseDate),
                _ => isDesc ? filmsQuery.OrderByDescending(s => s.Title) : filmsQuery.OrderBy(s => s.Title)
            };

            return new FilteredFilmsResult
            {
                Count = await filmsQuery.CountAsync(),
                Films = await filmsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync()
            };
        }

        public async Task<Film> GetByIdAsync(int id)
        {
            return await _db.Films
                .AsNoTracking().AsSplitQuery()
                .Include(f => f.Comments)
                    .ThenInclude(c => c.Author)
                .Include(c => c.Persons)
                    .ThenInclude(c => c.Person)
                .Include(c => c.Genres)
                    .ThenInclude(k => k.Genre)
                .Include(c => c.Ratings)
                    .ThenInclude(c => c.Source)
                .Include(r => r.MyRatings)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateAsync(Film film)
        {
            _db.Add(film);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Film film)
        {
            _db.Update(film);

            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Films.AnyAsync(e => e.Id == id);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var film = await GetByIdAsync(id);
            if (film != null)
            {
                _db.Films.Remove(film);
                await _db.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}