using System.Collections.Generic;

namespace Net_CampMyProject.Data.Models
{
    public class Person : DbEntityBase<int>
    {
        public string Name { get; set; }

        public string ImgUrl { get; set; }

        public string BirthnIformation { get; set; }

        public string Description { get; set; }

        public string SocialNetworksLinks { get; set; }

        public virtual ICollection<FilmPerson> FilmPersons { get; set; }
    }
}
