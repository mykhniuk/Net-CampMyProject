using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICommentsRepository _commentsRepository;

        public CommentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ICommentsRepository commentsRepository)
        {
            _context = context;
            _userManager = userManager;
            _commentsRepository = commentsRepository;
        }

        // GET: Comments
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            return View(await _commentsRepository.GetAllCommentsListAsync());
        }

        // GET: Comments/Details/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Details(int id)
        {
            var comment = await _commentsRepository.GetByIdAsync(id);

            if (comment == null)
                return NotFound();

            return View(comment);
        }

        // GET: Comments/Create
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Create()
        {
            InitializeSelectLists();

            return View();
        }

        private void InitializeSelectLists()
        {
            ViewData[nameof(Comment.AuthorId)] = new SelectList(_context.Users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewData[nameof(Comment.FilmId)] = new SelectList(_context.Films, nameof(Film.Id), nameof(Film.Id));
        }


        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.AuthorId = _userManager.GetUserId(User);
                comment.DateTime = DateTime.Now;

                await _commentsRepository.CreateAsync(comment);

                return RedirectToAction(nameof(Details), "Films", new {id = comment.FilmId});
            }

            
            InitializeSelectLists();

            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentsRepository.GetByIdAsync(id);
            InitializeSelectLists();

            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   await _commentsRepository.UpdateAsync(comment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _commentsRepository.ExistsAsync(comment.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            InitializeSelectLists();

            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentsRepository.GetByIdAsync(id);

            if (comment == null)
                return NotFound();

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _commentsRepository.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
