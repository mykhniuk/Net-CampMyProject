using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Net_CampMyProject.Data.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Content { get; set; }

        public string FilmId { get; set; }
        public virtual Film Film { get; set; }

        public string AuthorId { get; set; }
        public virtual IdentityUser Author { get; set; }
    }
}