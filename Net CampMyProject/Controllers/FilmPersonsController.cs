using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class FilmPersonsController : Controller
    {
        private readonly IFilmPersonsRepository _filmPersonRepository;
        private readonly IFilmsRepository _filmsRepository;
        private readonly IPersonsRepository _personsRepository;

        public FilmPersonsController(IFilmPersonsRepository filmPersonRepository, IFilmsRepository filmsRepository, IPersonsRepository personsRepository)
        {
            _filmPersonRepository = filmPersonRepository;
            _filmsRepository = filmsRepository;
            _personsRepository = personsRepository;
        }

        // GET: FilmPersons
        public async Task<IActionResult> Index()
        {
            return View(await _filmPersonRepository.GetAll().ToListAsync());
        }

        // GET: FilmPersons/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var filmPerson = await _filmPersonRepository.GetByIdAsync(id);
            if (filmPerson == null)
            {
                return NotFound();
            }

            return View(filmPerson);
        }

        // GET: FilmPersons/Create
        public IActionResult Create()
        {
            InitilizeSelectLists();
            return View();
        }

        private void InitilizeSelectLists(int? selectedPersonId = null, int? selectedFilmId = null)
        {
            ViewData[nameof(FilmPerson.PersonId)] = new SelectList(_personsRepository.GetAll(), nameof(Person.Id), nameof(Person.Name), selectedPersonId);
            ViewData[nameof(FilmPerson.FilmId)] = new SelectList(_filmsRepository.GetAll(), nameof(Film.Id), nameof(Film.Title), selectedFilmId);
        }

        // POST: FilmPersons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmPerson filmPerson)
        {
            if (ModelState.IsValid)
            {
                await _filmPersonRepository.CreateAsync(filmPerson);
                return RedirectToAction(nameof(Index));
            }

            InitilizeSelectLists(filmPerson.PersonId, filmPerson.FilmId);

            return View(filmPerson);
        }

        // GET: FilmPersons/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var filmPerson = await _filmPersonRepository.GetByIdAsync(id);
            if (filmPerson == null)
            {
                return NotFound();
            }

            InitilizeSelectLists(filmPerson.PersonId, filmPerson.FilmId);

            return View(filmPerson);
        }

        // POST: FilmPersons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FilmPerson filmPerson)
        {
            if (id != filmPerson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _filmPersonRepository.UpdateAsync(filmPerson);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _filmPersonRepository.ExistsAsync(filmPerson.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            InitilizeSelectLists(filmPerson.PersonId, filmPerson.FilmId);

            return View(filmPerson);
        }

        // GET: FilmPersons/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var filmPerson = await _filmPersonRepository.GetByIdAsync(id);
            if (filmPerson == null)
            {
                return NotFound();
            }

            return View(filmPerson);
        }

        // POST: FilmPersons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filmPerson = await _filmPersonRepository.GetByIdAsync(id);
            await _filmPersonRepository.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
