using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Net_CampMyProject.Data.Models
{
    public class Film : DbEntityBase<int>
    {
        public string Title { get; set; }

        public string ImgUrl { get; set; }

        public string Country { get; set; }       

        public string Duration { get; set; }

        [DisplayName("Trailer")]
        public string TrailerUrl { get; set; }

        public string Story { get; set; }

        public string WikiUrl { get; set; }

        public string Description { get; set; }

        [DisplayName("Release date")]
        public DateTime ReleaseDate { get; set; }

        public string Budget { get; set; }

        public string Languages { get; set; }

        public string Awards { get; set; }

        [DisplayName("Box office")]
        public string BoxOffice { get; set; }

        public string Nominations { get; set; }

        public virtual ICollection<FilmPerson> Persons { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<FilmRating> Ratings { get; set; }

        public virtual ICollection<FilmGenre> Genres { get; set; }

        public virtual ICollection<MyFilmRating> MyRatings { get; set; }
    }
}
