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

        public DbSet<MostPopularFilm> Films { get; set; }

        public DbSet<Comment> Comments { get; set; }
    }
}
