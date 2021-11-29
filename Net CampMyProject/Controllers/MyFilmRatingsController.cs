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
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    public class MyFilmRatingsController : Controller
    {
        private readonly IMyRatingsRepository _myRatingsRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFilmsRepository _filmsRepository;

        public MyFilmRatingsController(IMyRatingsRepository myRatingsRepository, UserManager<IdentityUser> userManager, IFilmsRepository filmsRepository)
        {
            _myRatingsRepository = myRatingsRepository;
            _userManager = userManager;
            _filmsRepository = filmsRepository;
        }

        // GET: MyFilmRatings
        public async Task<IActionResult> Index()
        {
            return View(await _myRatingsRepository.GetAll().ToListAsync());
        }

        // GET: MyFilmRatings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var myFilmRating = await _myRatingsRepository.GetByIdAsync(id);                
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
        public async Task<IActionResult> Create(MyFilmRating myFilmRating)
        {
            if (ModelState.IsValid)
            {
                await _myRatingsRepository.CreateAsync(myFilmRating);
                return RedirectToAction(nameof(Index));
            }

            InitializeSelectLists();
            return View(myFilmRating);
        }

        // GET: MyFilmRatings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var myFilmRating = await _myRatingsRepository.GetByIdAsync(id);
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
        public async Task<IActionResult> Edit(int id, MyFilmRating myFilmRating)
        {
            if (id != myFilmRating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _myRatingsRepository.UpdateAsync(myFilmRating);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _myRatingsRepository.ExistsAsync(myFilmRating.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            InitializeSelectLists();
            return View(myFilmRating);
        }
        private void InitializeSelectLists()
        {
            ViewData[nameof(MyFilmRating.AuthorId)] = new SelectList(_userManager.Users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewData[nameof(MyFilmRating.FilmId)] = new SelectList(_filmsRepository.GetAll(), nameof(Film.Id), nameof(Film.Title));
        }

        // GET: MyFilmRatings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var myFilmRating = await _myRatingsRepository.GetByIdAsync(id);
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
            var myFilmRating = await _myRatingsRepository.GetByIdAsync(id);
            await _myRatingsRepository.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateOrUpdate(MyFilmRating myFilmRating)
        {
            myFilmRating.AuthorId = _userManager.GetUserId(User);
            var dbMyFilmRating = await _myRatingsRepository.CreateOrUpdateAsync(myFilmRating);
            
                if (dbMyFilmRating == null)
                {

                    myFilmRating.AuthorId = _userManager.GetUserId(User);
                    await _myRatingsRepository.CreateAsync(myFilmRating);
                    return RedirectToAction(nameof(Details), "Films", new {id = myFilmRating.FilmId});

                }
                else
                {
                    dbMyFilmRating.AuthorId = myFilmRating.AuthorId;
                    dbMyFilmRating.MyRating = myFilmRating.MyRating;
                    await _myRatingsRepository.UpdateAsync(dbMyFilmRating);
                    return RedirectToAction(nameof(Details), "Films", new {id = dbMyFilmRating.FilmId});
                }
        }
        
    }
}
