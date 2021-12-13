using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;
using Net_CampMyProject.Services.Results;

namespace Net_CampMyProject.Services
{
    public class FilmsRepository : RepositoryBase<Film>, IFilmsRepository
    {
        public FilmsRepository(ApplicationDbContext db) : base(db)
        {

        }

        public async Task<FilteredFilmsResult> GetFilteredAsync(FilmsFilterType filterType, string sortBy, SortOrder sortOrder, int page, int pageSize, string authorId)
        {
            var filmsQueryBase = GetAll().AsSplitQuery()
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

        public async Task<List<Film>> SearchByKeyWordAsync(string keyWord)
        {
            return await GetAll().AsSplitQuery()
                .Include(f => f.Comments)
                .ThenInclude(c => c.Author)
                .Include(c => c.Persons)
                .ThenInclude(c => c.Person)
                .Include(c => c.Genres)
                .ThenInclude(k => k.Genre)
                .Include(c => c.Ratings)
                .ThenInclude(c => c.Source)
                .Include(r => r.MyRatings).Where(f => f.Title.Contains(keyWord) | f.Description.Contains(keyWord)).ToListAsync();
        }

        public override async Task<Film> GetByIdAsync(int id)
        {
            return await GetAll().AsSplitQuery()
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
    }
}