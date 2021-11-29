using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.ViewComponent
{
    public class RightPanelViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly IFilmsRepository _filmsRepository;

        public RightPanelViewComponent(IFilmsRepository filmsRepository)
        {
            _filmsRepository = filmsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _filmsRepository.GetAll().ToListAsync());
        }
    }
}
