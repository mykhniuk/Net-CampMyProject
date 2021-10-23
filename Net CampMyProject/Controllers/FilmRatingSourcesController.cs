using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;

namespace Net_CampMyProject.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class FilmRatingSourcesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilmRatingSourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FilmRatingSources
        public async Task<IActionResult> Index()
        {
            return View(await _context.FilmRatingSources.ToListAsync());
        }

        // GET: FilmRatingSources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmRatingSource = await _context.FilmRatingSources
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filmRatingSource == null)
            {
                return NotFound();
            }

            return View(filmRatingSource);
        }

        // GET: FilmRatingSources/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FilmRatingSources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmRatingSource filmRatingSource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(filmRatingSource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filmRatingSource);
        }

        // GET: FilmRatingSources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmRatingSource = await _context.FilmRatingSources.FindAsync(id);
            if (filmRatingSource == null)
            {
                return NotFound();
            }
            return View(filmRatingSource);
        }

        // POST: FilmRatingSources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FilmRatingSource filmRatingSource)
        {
            if (id != filmRatingSource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filmRatingSource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmRatingSourceExists(filmRatingSource.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(filmRatingSource);
        }

        // GET: FilmRatingSources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmRatingSource = await _context.FilmRatingSources
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filmRatingSource == null)
            {
                return NotFound();
            }

            return View(filmRatingSource);
        }

        // POST: FilmRatingSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filmRatingSource = await _context.FilmRatingSources.FindAsync(id);
            _context.FilmRatingSources.Remove(filmRatingSource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmRatingSourceExists(int id)
        {
            return _context.FilmRatingSources.Any(e => e.Id == id);
        }
    }
}
