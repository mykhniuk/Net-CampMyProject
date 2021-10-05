using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Models.ViewModels;

namespace Net_CampMyProject.Controllers
{
    
    public class MostPopularFilmsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MostPopularFilmsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: MostPopularFilms
        public async Task<IActionResult> Index(string sortBy = nameof(MostPopularFilm.ImbId), SortOrder sortOrder = SortOrder.Ascending, int takeCount = 10)
        {
            var filmsQuery = _db.Films.AsNoTracking().Select(f => new FimViewModel
            {
                ImbId = f.ImbId,
                Rank = f.Rank,
                Title = f.Title,
                FullTitle = f.FullTitle,
                Year = f.Year,
                Crew = f.Crew,
                ImDbRating = f.ImDbRating
            });

            if (sortOrder == SortOrder.Unspecified)
                sortOrder = SortOrder.Descending;

            var isDesc = sortOrder == SortOrder.Descending;

            filmsQuery = sortBy switch
            {
                nameof(MostPopularFilm.Rank) => isDesc ? filmsQuery.OrderByDescending(s => s.Rank) : filmsQuery.OrderBy(s => s.Rank),
                nameof(MostPopularFilm.Title) => isDesc ? filmsQuery.OrderByDescending(s => s.Title) : filmsQuery.OrderBy(s => s.Title),
                nameof(MostPopularFilm.FullTitle) => isDesc ? filmsQuery.OrderByDescending(s => s.FullTitle) : filmsQuery.OrderBy(s => s.FullTitle),
                nameof(MostPopularFilm.Year) => isDesc ? filmsQuery.OrderByDescending(s => s.Year) : filmsQuery.OrderBy(s => s.Year),
                nameof(MostPopularFilm.ImDbRating) => isDesc ? filmsQuery.OrderByDescending(s => s.ImDbRating) : filmsQuery.OrderBy(s => s.ImDbRating),
                _ => isDesc ? filmsQuery.OrderByDescending(s => s.ImbId) : filmsQuery.OrderBy(s => s.ImbId)
            };

            ViewBag.SortOrder = isDesc ? SortOrder.Ascending : SortOrder.Descending;

            return View(await filmsQuery.Take(takeCount).ToListAsync());
        }

        // GET: MostPopularFilms/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var film = await _db.Films.Include(f => f.Comments)
                                          .ThenInclude(c => c.Author)
                                      .FirstOrDefaultAsync(m => m.ImbId == id);

            if (film == null)
                return NotFound();

            return View(film);
        }

        // GET: MostPopularFilms/Create
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: MostPopularFilms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(MostPopularFilm film)
        {
            if (ModelState.IsValid)
            {
                _db.Add(film);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(film);
        }

        // GET: MostPopularFilms/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mostPopularFilm = await _db.Films.FindAsync(id);
            if (mostPopularFilm == null)
            {
                return NotFound();
            }
            return View(mostPopularFilm);
        }

        // POST: MostPopularFilms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(string id, MostPopularFilm film)
        {
            if (id != film.ImbId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(film);

                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MostPopularFilmExists(film.ImbId))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(film);
        }

        // GET: MostPopularFilms/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var film = await _db.Films
                .FirstOrDefaultAsync(m => m.ImbId == id);

            if (film == null)
                return NotFound();

            return View(film);
        }

        // POST: MostPopularFilms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var film = await _db.Films.FindAsync(id);
            if (film != null)
            {
                _db.Films.Remove(film);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MostPopularFilmExists(string id)
        {
            return _db.Films.Any(e => e.ImbId == id);
        }
    }
}
