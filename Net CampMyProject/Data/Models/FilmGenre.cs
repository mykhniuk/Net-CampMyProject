using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public class FilmGenre : DbEntityBase<int>
    {
        [Display(Name = "Genres")]
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }

        [Display(Name = "Film")]
        public int FilmId { get; set; }
        public virtual Film Film { get; set; }
    }
}
