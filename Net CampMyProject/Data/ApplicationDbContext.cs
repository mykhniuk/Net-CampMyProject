using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Film> Films { get; set; }

        public DbSet<FilmPerson> FilmPersons { get; set; }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<FilmRating> FilmRatings { get; set; }

        public DbSet<FilmRatingSource> FilmRatingSources { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<FilmGenre> FilmGenres { get; set; }

        public DbSet<MyFilmRating> MyFilmRating { get; set; }
    }
}
