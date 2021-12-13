using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class FilmGenresController : BaseController<FilmGenre>
    {
        private readonly IFilmsRepository _filmsRepository;
        private readonly IGenresRepository _genresRepository;

        public FilmGenresController(IFilmGenresRepository filmGenresRepository, IFilmsRepository filmsRepository, IGenresRepository genresRepository) : base(filmGenresRepository)
        {
            _filmsRepository = filmsRepository;
            _genresRepository = genresRepository;
        }
        
        public override void InitializeSelectLists()
        {
            ViewData[nameof(FilmGenre.GenreId)] = new SelectList(_genresRepository.GetAll(), nameof(Genre.Id), nameof(Genre.GenreType));
            ViewData[nameof(FilmGenre.FilmId)] = new SelectList(_filmsRepository.GetAll(), nameof(Film.Id), nameof(Film.Title));
        }
    }
}
