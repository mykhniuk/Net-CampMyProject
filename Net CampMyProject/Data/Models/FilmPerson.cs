using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public class FilmPerson
    {
        [Key]
        public int Id { get; set; }

        public FilmPersonRole Role { get; set; }

        [Display(Name = "Person")]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        [Display(Name = "Film")]
        public int FilmId { get; set; }
        public virtual Film Film { get; set; }
    }
}
