using System.Threading.Tasks;
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

        public FilmsController(UserManager<IdentityUser> userManager, IFilmsRepository filmsRepository) : base(filmsRepository)
        {
            _userManager = userManager;
            _filmsRepository = filmsRepository;
        }

        // GET: Films
        public async Task<IActionResult> List(FilmsFilterType filter, string sortBy = nameof(Film.Title), SortOrder sortOrder = SortOrder.Ascending, int page = 1, int pageSize = 5)
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

        public override async Task<IActionResult> Index()
        {
            return await Task.FromResult<IActionResult>(RedirectToAction(nameof(List), new {filter = FilmsFilterType.All}));
        }
    }
}