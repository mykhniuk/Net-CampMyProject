using System;
using Microsoft.AspNetCore.Identity;

namespace Net_CampMyProject.Data.Models
{
    public class Comment : DbEntityBase<int>
    {
        public DateTime DateTime { get; set; }

        public string Content { get; set; }

        public int FilmId { get; set; }
        public virtual Film Film { get; set; }

        public string AuthorId { get; set; }
        public virtual IdentityUser Author { get; set; }
    }
}