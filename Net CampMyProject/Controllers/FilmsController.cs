using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Models.ViewModels;

namespace Net_CampMyProject.Controllers
{
    
    public class FilmsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public FilmsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: Films
        public async Task<IActionResult> Index(FilmsFilterType filter, string sortBy = nameof(Film.Title), SortOrder sortOrder = SortOrder.Ascending, int page = 1, int pageSize = 5)
        {
            var authorId = _userManager.GetUserId(User);

            var filmsQueryBase = _db.Films
                .AsNoTracking().AsSplitQuery()
                .Include(c => c.MyRatings)
                .Include(c => c.Genres)
                     .ThenInclude(c => c.Genre)
                .Include(c => c.Ratings)
                     .ThenInclude(c => c.Source);

            var filmsQuery = filter switch
            {
                FilmsFilterType.All => filmsQueryBase,
                FilmsFilterType.Liked => filmsQueryBase.Where(f => f.MyRatings.Any(r => r.AuthorId == authorId && r.MyRating == true)),
                _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, "Unknown 'filter'")
            };

            if (sortOrder == SortOrder.Unspecified)
                sortOrder = SortOrder.Descending;

            var isDesc = sortOrder == SortOrder.Descending;

            filmsQuery = sortBy switch
            {
                nameof(Film.Title) => isDesc ? filmsQuery.OrderByDescending(s => s.Title) : filmsQuery.OrderBy(s => s.Title),
                nameof(Film.ReleaseDate) => isDesc ? filmsQuery.OrderByDescending(s => s.ReleaseDate) : filmsQuery.OrderBy(s => s.ReleaseDate),
                _ => isDesc ? filmsQuery.OrderByDescending(s => s.Title) : filmsQuery.OrderBy(s => s.Title)
            };

            var count = await filmsQuery.CountAsync();
            var items = await filmsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            
            var viewModel = new FilmsIndexViewModel
            {
                PaginationPageViewModel = new PaginationPageViewModel(count, page, pageSize),
                Films = items,
                Filter = filter,
                SortOrder = sortOrder,
                SortBy = sortBy,
            };

            return View(viewModel);
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var film = await _db.Films.Include(f=>f.Comments).ThenInclude(c=>c.Author)            
                                      .Include(c => c.Persons).ThenInclude(c => c.Person)
                                      .Include(c => c.Genres).ThenInclude(k=>k.Genre)
                                      .Include(c=>c.Ratings).ThenInclude(c=>c.Source)
                                      .Include(r=>r.MyRatings)
                                      .AsSplitQuery()
                                      .FirstOrDefaultAsync(m => m.Id == id);           

            if (film == null)
                return NotFound();

            return View(film);
        }

        // GET: Films/Create
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(Film film)
        {
            if (ModelState.IsValid)
            {
                _db.Add(film);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(film);
        }

        // GET: Films/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Film = await _db.Films.AsSplitQuery().Include(c=>c.Ratings).ThenInclude(c=>c.Source).FirstOrDefaultAsync(m => m.Id == id);
            if (Film == null)
            {
                return NotFound();
            }
            return View(Film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, Film film)
        {
            if (id != film.Id)
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
                    if (!FilmExists(film.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(film);
        }

        // GET: Films/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var film = await _db.Films
                .FirstOrDefaultAsync(m => m.Id == id);

            if (film == null)
                return NotFound();

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _db.Films.FindAsync(id);
            if (film != null)
            {
                _db.Films.Remove(film);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _db.Films.Any(e => e.Id == id);
        }
        public async Task<List<Film>>GetLikedFilms()
        {
            var authorId = _userManager.GetUserId(User);
            var filmsQuery = _db.Films
                .Include(c=>c.MyRatings)
                .Include(c=>c.Genres).ThenInclude(c=>c.Genre)
                .Include(c=>c.Ratings).ThenInclude(c=>c.Source).AsNoTracking().Where(f => f.MyRatings.FirstOrDefault(r => r.AuthorId == authorId && r.MyRating == true) != null).ToListAsync();
            return await filmsQuery;
        }
    }

    public enum FilmsFilterType
    {
        All,
        Liked
    }
}

