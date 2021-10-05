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

namespace Net_CampMyProject.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.Author).Include(c => c.Film);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Film)
                .FirstOrDefaultAsync(m => m.Id == id);

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
            ViewData[nameof(Comment.FilmId)] = new SelectList(_context.Films, nameof(MostPopularFilm.ImbId), nameof(MostPopularFilm.FullTitle));
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

                _context.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), "MostPopularFilms", new {id = comment.FilmId});
            }

            InitializeSelectLists();

            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

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
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Film)
                .FirstOrDefaultAsync(m => m.Id == id);

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
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
