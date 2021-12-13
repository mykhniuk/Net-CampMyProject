using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class FilmRatingsController : BaseController<FilmRating>
    {
        private readonly IFilmsRepository _filmsRepository;
        private readonly IFilmRatingSourcesRepository _filmRatingSourcesRepository;

        public FilmRatingsController(IFilmRatingsRepository filmRatingsRepository, IFilmRatingSourcesRepository filmRatingSourcesRepository, IFilmsRepository filmsRepository) : base(filmRatingsRepository)
        {
            _filmRatingSourcesRepository = filmRatingSourcesRepository;
            _filmsRepository = filmsRepository;
        }

        public override void InitializeSelectLists()
        {
            ViewData[nameof(FilmRating.SourceId)] = new SelectList(_filmRatingSourcesRepository.GetAll(), nameof(FilmRatingSource.Id), nameof(FilmRatingSource.ResourceWebsite));
            ViewData[nameof(FilmRating.FilmId)] = new SelectList(_filmsRepository.GetAll(), nameof(Film.Id), nameof(Film.Title));
        }
    }
}
