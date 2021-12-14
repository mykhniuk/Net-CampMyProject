using System;
using System.Threading.Tasks;
using AspNetCore.Unobtrusive.Ajax;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Models.ViewModels;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    public class FilmsController : BaseController<Film>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFilmsRepository _filmsRepository;
        private readonly ICommentsRepository _commentsRepository;

        public FilmsController(UserManager<IdentityUser> userManager, IFilmsRepository filmsRepository, ICommentsRepository commentsRepository) : base(filmsRepository)
        {
            _userManager = userManager;
            _filmsRepository = filmsRepository;
            _commentsRepository = commentsRepository;
        }

        // GET: Films
        [AllowAnonymous]
        public async Task<IActionResult> List(FilmsFilterType filter, string sortBy = nameof(Film.Title), SortOrder sortOrder = SortOrder.Ascending, int page = 1, int pageSize = 5, string keyWord = null)
        {
            var viewModel = new FilmsIndexViewModel();
            var authorId = _userManager.GetUserId(User);

            var filteredFilmsResult = await _filmsRepository.GetFilteredAsync(filter, sortBy, sortOrder, page, pageSize, authorId);
            if (keyWord == null)
            {
                viewModel = new FilmsIndexViewModel
                {
                    PaginationPageViewModel = new PaginationPageViewModel(filteredFilmsResult.Count, page, pageSize),
                    Films = filteredFilmsResult.Films,
                    Filter = filter,
                    SortOrder = sortOrder,
                    SortBy = sortBy,
                };
            }

            if (keyWord == null) return View(viewModel);
            viewModel.Films = await _filmsRepository.SearchByKeyWordAsync(keyWord);
            viewModel.Filter = FilmsFilterType.All;
            viewModel.SortOrder = SortOrder.Ascending;
            viewModel.PaginationPageViewModel =
                new PaginationPageViewModel(viewModel.Films.Count, page, pageSize);


            return View(viewModel);
        }

        [AllowAnonymous]
        public override async Task<IActionResult> Index()
        {
            return await Task.FromResult<IActionResult>(RedirectToAction(nameof(List), new {filter = FilmsFilterType.All}));
        }

        [AllowAnonymous]
        public override Task<IActionResult> Details(int id)
        {
            return base.Details(id);
        }

        [AjaxOnly]
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            comment.AuthorId = _userManager.GetUserId(User);
            comment.DateTime = DateTime.Now;

            await _commentsRepository.CreateAsync(comment);

            return RedirectToAction("VotesList", new  { id = comment.FilmId});
        }

        public async Task<ActionResult> VotesList(int id)
        {
            var comments = await _commentsRepository.GetByFilmIdAsync(id);
            if (comments == null)
                return NotFound();

            return PartialView("_VotesList", comments);
        }
    }
}