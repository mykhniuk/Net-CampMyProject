using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    public class CommentsController : BaseController<Comment>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFilmsRepository _filmsRepository;
        private readonly ICommentsRepository _commentsRepository;

        public CommentsController(IFilmsRepository filmsRepository, UserManager<IdentityUser> userManager, ICommentsRepository commentsRepository) : base(commentsRepository)
        {
            _userManager = userManager;
            _filmsRepository = filmsRepository;
            _commentsRepository = commentsRepository;
        }

        // GET: Comments
        [Authorize(Roles = Roles.Admin)]
        public override async Task<IActionResult> Index()
        {
            return View(await _commentsRepository.GetAllCommentsListAsync());
        }

        public override void InitializeSelectLists()
        {
            ViewData[nameof(Comment.AuthorId)] = new SelectList(_userManager.Users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewData[nameof(Comment.FilmId)] = new SelectList(_filmsRepository.GetAll().Select(f => new { f.Id, f.Title }), nameof(Film.Id), nameof(Film.Title));
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public override async Task<IActionResult> Create(Comment comment)
        {
            if (comment != null)
            {
                comment.AuthorId = _userManager.GetUserId(User);
                comment.DateTime = DateTime.Now;
            }

            return await base.Create(comment);
        }
    }
}
