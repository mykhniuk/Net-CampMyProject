using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public class FilmRating : DbEntityBase<int>
    {
        public string Rating { get; set; }

        [Display(Name = "Source")]
        public int SourceId { get; set; }
        public virtual FilmRatingSource Source { get; set; }

        [Display(Name = "Film")]
        public int FilmId { get; set; }
        public virtual Film Film { get; set; }
    }
}
