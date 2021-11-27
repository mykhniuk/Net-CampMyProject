using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Net_CampMyProject.Controllers;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Results;

namespace Net_CampMyProject.Services.Interfaces
{
    public interface IFilmsRepository
    {
        Task<FilteredFilmsResult> GetFilteredAsync(FilmsFilterType filterType, string sortBy, SortOrder sortOrder, int page, int pageSize, string authorId);
        Task<Film> GetByIdAsync(int id);
        Task CreateAsync(Film film);
        Task UpdateAsync(Film film);
        Task<bool> ExistsAsync(int id);
        Task DeleteByIdAsync(int id);
    }
}
