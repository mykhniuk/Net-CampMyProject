using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    [Authorize]
    public class PersonsController : Controller
    {
        private readonly IPersonsRepository _personRepository;

        public PersonsController(IPersonsRepository personRepository)
        {
            _personRepository = personRepository;
        }

        // GET: Persons
        public async Task<IActionResult> Index()
        {
            return View(await _personRepository.GetAll().ToListAsync());
        }

        // GET: Persons/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var film = await _personRepository.GetByIdAsync(id);
            if (film == null)
                return NotFound();

            return View(film);
        }

        // GET: Persons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Persons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person)
        {
            if (ModelState.IsValid)
            {
                await _personRepository.CreateAsync(person);
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: Persons/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: Persons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _personRepository.UpdateAsync(person);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _personRepository.ExistsAsync(person.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: Persons/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Persons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            await _personRepository.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
