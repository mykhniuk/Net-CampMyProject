using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class FilmPersonsController : BaseController<FilmPerson>
    {
        private readonly IFilmsRepository _filmsRepository;
        private readonly IPersonsRepository _personsRepository;

        public FilmPersonsController(IFilmPersonsRepository filmPersonRepository, IFilmsRepository filmsRepository, IPersonsRepository personsRepository) : base(filmPersonRepository)
        {
            _filmsRepository = filmsRepository;
            _personsRepository = personsRepository;
        }

        public override void InitializeSelectLists()
        {
            ViewData[nameof(FilmPerson.PersonId)] = new SelectList(_personsRepository.GetAll(), nameof(Person.Id), nameof(Person.Name));
            ViewData[nameof(FilmPerson.FilmId)] = new SelectList(_filmsRepository.GetAll(), nameof(Film.Id), nameof(Film.Title));
        }
    }
}
