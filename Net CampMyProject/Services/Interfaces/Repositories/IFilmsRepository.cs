using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Results;

namespace Net_CampMyProject.Services.Interfaces
{
    public interface IFilmsRepository : IRepositoryBase<Film>
    {
        Task<FilteredFilmsResult> GetFilteredAsync(FilmsFilterType filterType, string sortBy, SortOrder sortOrder, int page, int pageSize, string authorId);
        Task<List<Film>> SearchByKeyWordAsync(string keyWord);
    }
}
