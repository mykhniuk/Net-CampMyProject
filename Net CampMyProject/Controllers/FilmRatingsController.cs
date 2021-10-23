using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;

namespace Net_CampMyProject.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class FilmRatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilmRatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FilmRatings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FilmRatings.Include(f => f.Source);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FilmRatings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmRating = await _context.FilmRatings
                .Include(f => f.Source)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            ViewData[nameof(FilmRating.SourceId)] = new SelectList(_context.FilmRatingSources, nameof(FilmRatingSource.Id), nameof(FilmRatingSource.ResourceWebsite), selectedRatingSourceId);
            ViewData[nameof(FilmRating.FilmId)] = new SelectList(_context.Films, nameof(Film.Id), nameof(Film.Title), selectedFilmId);
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
                _context.Add(filmRating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            InitilizeSelectLists(filmRating.SourceId, filmRating.FilmId);

            return View(filmRating);
        }

        // GET: FilmRatings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmRating = await _context.FilmRatings.FindAsync(id);
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
                    _context.Update(filmRating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmRatingExists(filmRating.Id))
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

            InitilizeSelectLists(filmRating.SourceId, filmRating.FilmId);

            return View(filmRating);
        }

        // GET: FilmRatings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmRating = await _context.FilmRatings
                .Include(f => f.Source)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var filmRating = await _context.FilmRatings.FindAsync(id);
            _context.FilmRatings.Remove(filmRating);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmRatingExists(int id)
        {
            return _context.FilmRatings.Any(e => e.Id == id);
        }
    }
}
