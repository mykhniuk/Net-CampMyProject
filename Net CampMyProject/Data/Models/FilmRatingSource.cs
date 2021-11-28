using System.Collections.Generic;

namespace Net_CampMyProject.Data.Models
{
    public class FilmRatingSource : DbEntityBase<int>
    {
        public string ResourceWebsite { get; set; }

        public virtual ICollection<FilmRating> FilmRatings { get; set; }
    }
}
