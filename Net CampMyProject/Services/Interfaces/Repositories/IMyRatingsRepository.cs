using System.Threading.Tasks;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Services.Interfaces
{
    public interface IMyRatingsRepository : IRepositoryBase<MyFilmRating>
    {
        Task<MyFilmRating> CreateOrUpdateAsync(MyFilmRating myFilmRating);
    }
}
