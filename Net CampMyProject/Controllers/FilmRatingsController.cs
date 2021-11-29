using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class FilmRatingsController : Controller
    {
        private readonly IFilmRatingsRepository _filmRatingsRepository;
        private readonly IFilmsRepository _filmsRepository;
        private readonly IFilmRatingSourcesRepository _filmRatingSourcesRepository;

        public FilmRatingsController(IFilmRatingsRepository filmRatingsRepository, IFilmRatingSourcesRepository filmRatingSourcesRepository, IFilmsRepository filmsRepository)
        {
            _filmRatingsRepository = filmRatingsRepository;
            _filmRatingSourcesRepository = filmRatingSourcesRepository;
            _filmsRepository = filmsRepository;
        }

        // GET: FilmRatings
        public async Task<IActionResult> Index()
        {
            return View(await _filmRatingsRepository.GetAll().Include(c=>c.Source).ToListAsync());
        }

        // GET: FilmRatings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var filmRating = await _filmRatingsRepository.GetByIdAsync(id);
            if (filmRating == null)
            {
                return NotFound();
            }
            return View(filmRating);
        }

        // GET: FilmRatings/Create
        public IActionResult Create()
        {
            InitilizeSelectLists();

            return View();
        }

        private void InitilizeSelectLists(int? selectedRatingSourceId = null, int? selectedFilmId = null)
        {
            ViewData[nameof(FilmRating.SourceId)] = new SelectList(_filmRatingSourcesRepository.GetAll(), nameof(FilmRatingSource.Id), nameof(FilmRatingSource.ResourceWebsite), selectedRatingSourceId);
            ViewData[nameof(FilmRating.FilmId)] = new SelectList(_filmsRepository.GetAll(), nameof(Film.Id), nameof(Film.Title), selectedFilmId);
        }

        // POST: FilmRatings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmRating filmRating)
        {
            if (ModelState.IsValid)
            {
                await _filmRatingsRepository.CreateAsync(filmRating);
                return RedirectToAction(nameof(Index));
            }
            InitilizeSelectLists(filmRating.SourceId, filmRating.FilmId);

            return View(filmRating);
        }

        // GET: FilmRatings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var filmRating = await _filmRatingsRepository.GetByIdAsync(id);
            if (filmRating == null)
            {
                return NotFound();
            }

            InitilizeSelectLists(filmRating.SourceId, filmRating.FilmId);

            return View(filmRating);
        }

        // POST: FilmRatings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FilmRating filmRating)
        {
            if (id != filmRating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _filmRatingsRepository.UpdateAsync(filmRating);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _filmRatingsRepository.ExistsAsync(filmRating.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            InitilizeSelectLists(filmRating.SourceId, filmRating.FilmId);

            return View(filmRating);
        }

        // GET: FilmRatings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var filmRating = await _filmRatingsRepository.GetByIdAsync(id);
            if (filmRating == null)
            {
                return NotFound();
            }

            return View(filmRating);
        }

        // POST: FilmRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filmRating = await _filmRatingsRepository.GetByIdAsync(id);
            await _filmRatingsRepository.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
