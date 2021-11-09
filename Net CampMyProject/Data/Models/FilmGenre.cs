using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Net_CampMyProject.Data.Models
{
    public class FilmGenre
    {
        [Key]
        public int Id { get; set; }       

        [Display(Name = "Genre")]
        public int GenrenId { get; set; }
        public virtual Genre Genre { get; set; }

        [Display(Name = "Film")]
        public int FilmId { get; set; }
        public virtual Film Film { get; set; }
    }
}
