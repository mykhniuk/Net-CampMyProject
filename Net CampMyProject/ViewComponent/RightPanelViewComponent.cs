using System;
using System.Collections.Generic;
using System.Linq;
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
        
        var topFilms = new List<TopFilmsViewModel>();
            var allFilms = await _filmsRepository.GetAll().Include(r=>r.Ratings).ToListAsync();
           foreach (var film in allFilms)
           {
               var newTopFilm = new TopFilmsViewModel();
                foreach (var r in film.Ratings)
                {
                    var stringRating = r.Rating.Split("/").ElementAt(0);
                    _ = double.TryParse(stringRating, out var parseStringRating);
                    if (parseStringRating == 0)
                    {
                        var rating = r.Rating.Replace("%","");
                        _ = double.TryParse(rating, out parseStringRating);
                        parseStringRating /= 10.0;
                    }
                    newTopFilm.Rating += parseStringRating /film.Ratings.Count;
                    newTopFilm.Rating = Math.Round(newTopFilm.Rating, 1);

                }
                newTopFilm.Film = film;
                topFilms.Add(newTopFilm);
            }
            return View("Default1", topFilms.OrderByDescending(t=>t.Rating).Take(10));
        }
    }
}
