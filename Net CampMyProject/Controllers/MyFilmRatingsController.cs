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
    public class MyFilmRatingsController : BaseController<MyFilmRating>
    {
        private readonly IMyRatingsRepository _myRatingsRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFilmsRepository _filmsRepository;

        public MyFilmRatingsController(IMyRatingsRepository myRatingsRepository, UserManager<IdentityUser> userManager, IFilmsRepository filmsRepository) : base(myRatingsRepository)
        {
            _myRatingsRepository = myRatingsRepository;
            _userManager = userManager;
            _filmsRepository = filmsRepository;
        }

        public override void InitializeSelectLists()
        {
            ViewData[nameof(MyFilmRating.AuthorId)] = new SelectList(_userManager.Users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewData[nameof(MyFilmRating.FilmId)] = new SelectList(_filmsRepository.GetAll(), nameof(Film.Id), nameof(Film.Title));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(MyFilmRating myFilmRating)
        {
            myFilmRating.AuthorId = _userManager.GetUserId(User);

            await _myRatingsRepository.CreateOrUpdateAsync(myFilmRating);

            return RedirectToAction(nameof(Details), "Films", new {id = myFilmRating.FilmId });
        }
    }
}
