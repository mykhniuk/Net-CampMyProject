using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Controllers
{
    public class MyFilmRatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyFilmRatingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MyFilmRatings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MyFilmRating.Include(m => m.Author).Include(m => m.Film);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MyFilmRatings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myFilmRating = await _context.MyFilmRating
                .Include(m => m.Author)
                .Include(m => m.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myFilmRating == null)
            {
                return NotFound();
            }

            return View(myFilmRating);
        }

        // GET: MyFilmRatings/Create
        public IActionResult Create()
        {
            InitializeSelectLists();
            return View();
        }

        // POST: MyFilmRatings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MyRating,FilmId,AuthorId")] MyFilmRating myFilmRating)
        {
            if (ModelState.IsValid)
            {
                _context.Add(myFilmRating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            InitializeSelectLists();
            return View(myFilmRating);
        }

        // GET: MyFilmRatings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myFilmRating = await _context.MyFilmRating.FindAsync(id);
            if (myFilmRating == null)
            {
                return NotFound();
            }

            InitializeSelectLists();
            return View(myFilmRating);
        }

        // POST: MyFilmRatings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MyRating,FilmId,AuthorId")] MyFilmRating myFilmRating)
        {
            if (id != myFilmRating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(myFilmRating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyFilmRatingExists(myFilmRating.Id))
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

            InitializeSelectLists();
            return View(myFilmRating);
        }
        private void InitializeSelectLists()
        {
            ViewData[nameof(MyFilmRating.AuthorId)] = new SelectList(_context.Users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewData[nameof(MyFilmRating.FilmId)] = new SelectList(_context.Films, nameof(Film.Id), nameof(Film.Id));
        }

        // GET: MyFilmRatings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myFilmRating = await _context.MyFilmRating
                .Include(m => m.Author)
                .Include(m => m.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (myFilmRating == null)
            {
                return NotFound();
            }

            return View(myFilmRating);
        }

        // POST: MyFilmRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myFilmRating = await _context.MyFilmRating.FindAsync(id);
            _context.MyFilmRating.Remove(myFilmRating);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyFilmRatingExists(int id)
        {
            return _context.MyFilmRating.Any(e => e.Id == id);
        }
        public async Task<IActionResult> CreateOrUpdate([Bind("Id,MyRating,FilmId,AuthorId")] MyFilmRating myFilmRating)
        {
            myFilmRating.AuthorId = _userManager.GetUserId(User);
            var dbMyFilmRating = await _context.MyFilmRating.Include(c => c.Film).Include(a => a.Author).FirstOrDefaultAsync(mr => mr.FilmId == myFilmRating.FilmId && mr.AuthorId == myFilmRating.AuthorId);
            
                if (dbMyFilmRating == null)
                {

                    myFilmRating.AuthorId = _userManager.GetUserId(User);
                    _context.Add(myFilmRating);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), "Films", new {id = myFilmRating.FilmId});

                }
                else
                {
                    dbMyFilmRating.AuthorId = myFilmRating.AuthorId;
                    dbMyFilmRating.MyRating = myFilmRating.MyRating;
                    _context.Update(dbMyFilmRating);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), "Films", new {id = dbMyFilmRating.FilmId});
                }

            InitializeSelectLists();

            return View(myFilmRating);
        }
        
    }
}
