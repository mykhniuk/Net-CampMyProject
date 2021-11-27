using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Models.ViewModels;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    public class FilmsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFilmsRepository _filmsRepository;

        public FilmsController(UserManager<IdentityUser> userManager, IFilmsRepository filmsRepository)
        {
            _userManager = userManager;
            _filmsRepository = filmsRepository;
        }

        // GET: Films
        public async Task<IActionResult> Index(FilmsFilterType filter, string sortBy = nameof(Film.Title), SortOrder sortOrder = SortOrder.Ascending, int page = 1, int pageSize = 5)
        {
            var authorId = _userManager.GetUserId(User);

            var filteredFilmsResult = await _filmsRepository.GetFilteredAsync(filter, sortBy, sortOrder, page, pageSize, authorId);

            var viewModel = new FilmsIndexViewModel
            {
                PaginationPageViewModel = new PaginationPageViewModel(filteredFilmsResult.Count, page, pageSize),
                Films = filteredFilmsResult.Films,
                Filter = filter,
                SortOrder = sortOrder,
                SortBy = sortBy,
            };

            return View(viewModel);
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var film = await _filmsRepository.GetByIdAsync(id);
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
                await _filmsRepository.CreateAsync(film);
                return RedirectToAction(nameof(Index));
            }

            return View(film);
        }

        // GET: Films/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var film = await _filmsRepository.GetByIdAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
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
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _filmsRepository.UpdateAsync(film);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _filmsRepository.ExistsAsync(film.Id))
                    {
                        return NotFound();
                    }

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
            var film = await _filmsRepository.GetByIdAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           await _filmsRepository.DeleteByIdAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}