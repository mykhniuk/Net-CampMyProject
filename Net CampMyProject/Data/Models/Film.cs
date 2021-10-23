using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public class Film
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImgUrl { get; set; }

        public string Duration { get; set; }

        public string TrailerUrl { get; set; }

        public string Description { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Budget { get; set; }

        public string Languages { get; set; }

        public string Awards { get; set; }

        public string Screenplay { get; set; }

        public string BoxOffice { get; set; }

        public string Nominations { get; set; }

        public virtual ICollection<FilmPerson> Persons { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<FilmRating> Ratings { get; set; }
    }
}
