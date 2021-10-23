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
    public class FilmPersonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilmPersonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FilmPersons
        public async Task<IActionResult> Index()
        {
            return View(await _context.FilmPersons.ToListAsync());
        }

        // GET: FilmPersons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmPerson = await _context.FilmPersons
                .FirstOrDefaultAsync(m => m.Id == id);
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
            ViewData[nameof(FilmPerson.PersonId)] = new SelectList(_context.Persons, nameof(Person.Id), nameof(Person.Name), selectedPersonId);
            ViewData[nameof(FilmPerson.FilmId)] = new SelectList(_context.Films, nameof(Film.Id), nameof(Film.Title), selectedFilmId);
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
                _context.Add(filmPerson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            InitilizeSelectLists(filmPerson.PersonId, filmPerson.FilmId);

            return View(filmPerson);
        }

        // GET: FilmPersons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmPerson = await _context.FilmPersons.FindAsync(id);
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
                    _context.Update(filmPerson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmPersonExists(filmPerson.Id))
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

            InitilizeSelectLists(filmPerson.PersonId, filmPerson.FilmId);

            return View(filmPerson);
        }

        // GET: FilmPersons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmPerson = await _context.FilmPersons
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var filmPerson = await _context.FilmPersons.FindAsync(id);
            _context.FilmPersons.Remove(filmPerson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmPersonExists(int id)
        {
            return _context.FilmPersons.Any(e => e.Id == id);
        }
    }
}
