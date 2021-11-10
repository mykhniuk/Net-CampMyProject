using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Net_CampMyProject.Data.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        public string GenreType { get; set; }

        public virtual ICollection<FilmGenre> FilmGenres { get; set; }
    }
}
