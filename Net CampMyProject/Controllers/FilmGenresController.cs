using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    public class FilmGenresController : Controller
    {
        private readonly IFilmGenresRepository _filmGenresRepository;
        private readonly IFilmsRepository _filmsRepository;
        private readonly IGenresRepository _genresRepository;

        public FilmGenresController(IFilmGenresRepository filmGenresRepository, IFilmsRepository filmsRepository, IGenresRepository genresRepository)
        {
            _filmGenresRepository = filmGenresRepository;
            _filmsRepository = filmsRepository;
            _genresRepository = genresRepository;
        }

        // GET: FilmGenres
        public async Task<IActionResult> Index()
        {
            return View(await _filmGenresRepository.GetAll().ToListAsync());
        }

        // GET: FilmGenres/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var filmGenre = await _filmGenresRepository.GetByIdAsync(id);
            if (filmGenre == null)
            {
                return NotFound();
            }

            return View(filmGenre);
        }

        // GET: FilmGenres/Create
        public IActionResult Create()
        {
            InitilizeSelectLists();
            return View();
        }
        private void InitilizeSelectLists(int? selectedGenreId = null, int? selectedFilmId = null)
        {
            ViewData[nameof(FilmGenre.GenreId)] = new SelectList(_genresRepository.GetAll(), nameof(Genre.Id), nameof(Genre.GenreType), selectedGenreId);
            ViewData[nameof(FilmGenre.FilmId)] = new SelectList(_filmsRepository.GetAll(), nameof(Film.Id), nameof(Film.Title), selectedFilmId);
        }

        // POST: FilmGenres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmGenre filmGenre)
        {
            if (ModelState.IsValid)
            {
                await _filmGenresRepository.CreateAsync(filmGenre);
                return RedirectToAction(nameof(Index));
            }
            InitilizeSelectLists();
            return View(filmGenre);
        }

        // GET: FilmGenres/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var filmGenre = await _filmGenresRepository.GetByIdAsync(id);
            if (filmGenre == null)
            {
                return NotFound();
            }
            InitilizeSelectLists();
            return View(filmGenre);
        }

        // POST: FilmGenres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FilmGenre filmGenre)
        {
            if (id != filmGenre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _filmGenresRepository.UpdateAsync(filmGenre);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _filmsRepository.ExistsAsync(filmGenre.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            InitilizeSelectLists();
            return View(filmGenre);
        }

        // GET: FilmGenres/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var filmGenre = await _filmGenresRepository.GetByIdAsync(id);
            if (filmGenre == null)
            {
                return NotFound();
            }

            return View(filmGenre);
        }

        // POST: FilmGenres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filmGenre = await _filmGenresRepository.GetByIdAsync(id);
            await _filmGenresRepository.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
