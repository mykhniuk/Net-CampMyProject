using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public class MyFilmRating
    {
        [Key]
        public int Id { get; set; }

        public bool MyRating { get; set; }

        public int FilmId { get; set; }
        public virtual Film Film { get; set; }

        public string AuthorId { get; set; }
        public virtual IdentityUser Author { get; set; }
    }
}
