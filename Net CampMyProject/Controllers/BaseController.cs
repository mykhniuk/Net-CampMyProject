using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    public abstract class BaseController<TEntity> : Controller where TEntity : DbEntityBase<int>
    {
        private readonly IRepositoryBase<TEntity> _repository;

        protected BaseController(IRepositoryBase<TEntity> repository)
        {
            _repository = repository;
        }

        // GET: <Entities>
        public virtual async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll().ToListAsync());
        }

        // GET: <Entities>/Details/5
        public virtual async Task<IActionResult> Details(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // GET: <Entities>/Create
        public virtual IActionResult Create()
        {
            InitializeSelectLists();

            return View();
        }

        public virtual void InitializeSelectLists()
        {
        }

        // POST: <Entities>/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TEntity entity)
        {
            if (ModelState.IsValid)
            {
                await _repository.CreateAsync(entity);
                return RedirectToAction(nameof(Index));
            }

            InitializeSelectLists();

            return View(entity);
        }

        // GET: <Entities>/Edit/5
        public virtual async Task<IActionResult> Edit(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            InitializeSelectLists();

            return View(entity);
        }

        // POST: <Entities>/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, TEntity entity)
        {
            if (id != entity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(entity);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _repository.ExistsAsync(entity.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            InitializeSelectLists();

            return View(entity);
        }

        // GET: <Entities>/Delete/5
        public virtual async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // POST: <Entities>/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
