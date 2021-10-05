using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public class MostPopularFilm
    {
        [Key]
        public string ImbId { get; set; }

        public string Rank { get; set; }

        public string RankUpDown { get; set; }

        public string Title { get; set; }

        public string FullTitle { get; set; }

        public string Year { get; set; }

        public string ImageUrl { get; set; }

        public string Crew { get; set; }

        public string ImDbRating { get; set; }

        public string ImDbRatingCount { get; set; }

        public string TrailerUrl { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }  
    }
}
