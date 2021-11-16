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
    
    public class FilmsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FilmsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Films
        public async Task<IActionResult> Index(string sortBy = nameof(Film.Title), SortOrder sortOrder = SortOrder.Ascending, int takeCount = 20, int page=1)
        {
           int pageSize = 5;
           
           var filmsQuery = _db.Films.AsNoTracking()
               .Include(g => g.Genres).ThenInclude(g => g.Genre).AsNoTracking()
               .Include(r => r.Ratings).ThenInclude(s => s.Source).AsNoTracking()
               .Include(m => m.MyRatings).AsNoTracking().AsSplitQuery();

            if (sortOrder == SortOrder.Unspecified)
                sortOrder = SortOrder.Descending;

            var isDesc = sortOrder == SortOrder.Descending;

            filmsQuery = sortBy switch
            {
                nameof(Film.Title) => isDesc ? filmsQuery.OrderByDescending(s => s.Title) : filmsQuery.OrderBy(s => s.Title),
                nameof(Film.ReleaseDate) => isDesc ? filmsQuery.OrderByDescending(s => s.ReleaseDate) : filmsQuery.OrderBy(s => s.ReleaseDate),
                _ => isDesc ? filmsQuery.OrderByDescending(s => s.Title) : filmsQuery.OrderBy(s => s.Title)
            };

            ViewBag.SortOrder = isDesc ? SortOrder.Ascending : SortOrder.Descending;
            ViewData["Message"] = sortBy;

            var count = await filmsQuery.CountAsync();
            var items = await filmsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Films = items
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
        public async Task<IActionResult> Index1(int page = 1)
        {
            int pageSize = 3;   // количество элементов на странице

            IQueryable<Film> source = _db.Films.Include(f => f.Comments).ThenInclude(c => c.Author)
                .Include(c => c.Persons).ThenInclude(c => c.Person)
                .Include(c => c.Genres).ThenInclude(k => k.Genre)
                .Include(c => c.Ratings).ThenInclude(c => c.Source)
                .Include(r => r.MyRatings)
                .AsSplitQuery();
            var count = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Films = items
            };
            return View(viewModel);
        }
    }
}

