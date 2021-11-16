using System;
using System.Collections.Generic;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Models.ViewModels
{
    public class FimViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime ReleaseDate { get; set; }

        public  string ImgUrl { get; set; }

        public  string Genres { get; set; }

        public  string Duration { get; set; }

        public string Description { get; set; }

        public ICollection<FilmRating> Ratings { get; set; }

        public string Country { get; set; }

        public ICollection<MyFilmRating> MyFilmRatins { get; set; }

    }
}
