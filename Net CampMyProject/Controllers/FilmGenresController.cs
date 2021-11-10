using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Controllers
{
    public class FilmGenresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilmGenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FilmGenres
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FilmGenres.Include(f => f.Film);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FilmGenres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmGenre = await _context.FilmGenres
                .Include(f => f.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            ViewData[nameof(FilmGenre.GenreId)] = new SelectList(_context.Genres, nameof(Genre.Id), nameof(Genre.GenreType), selectedGenreId);
            ViewData[nameof(FilmGenre.FilmId)] = new SelectList(_context.Films, nameof(Film.Id), nameof(Film.Title), selectedFilmId);
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
                _context.Add(filmGenre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            InitilizeSelectLists();
            return View(filmGenre);
        }

        // GET: FilmGenres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmGenre = await _context.FilmGenres.FindAsync(id);
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
                    _context.Update(filmGenre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmGenreExists(filmGenre.Id))
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
            InitilizeSelectLists();
            return View(filmGenre);
        }

        // GET: FilmGenres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmGenre = await _context.FilmGenres
                .Include(f => f.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var filmGenre = await _context.FilmGenres.FindAsync(id);
            _context.FilmGenres.Remove(filmGenre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmGenreExists(int id)
        {
            return _context.FilmGenres.Any(e => e.Id == id);
        }
    }
}
