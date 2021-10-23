﻿using System.ComponentModel.DataAnnotations;

namespace Net_CampMyProject.Data.Models
{
    public class FilmRatingSource
    {
        [Key]
        public int Id { get; set; }

        public string ResourceWebsite { get; set; }
    }
}
