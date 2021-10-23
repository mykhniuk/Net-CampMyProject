using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImgUrl { get; set; }

        public string BirthnIformation { get; set; }

        public string Description { get; set; }

        public string SocialNetworksLinks { get; set; }

        public virtual ICollection<FilmPerson> FilmPersons { get; set; }
    }
}
