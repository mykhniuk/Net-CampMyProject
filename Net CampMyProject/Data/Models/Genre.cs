using System.Collections.Generic;

namespace Net_CampMyProject.Data.Models
{
    public class Genre : DbEntityBase<int>
    {
        public string GenreType { get; set; }

        public virtual ICollection<FilmGenre> FilmGenres { get; set; }
    }
}
