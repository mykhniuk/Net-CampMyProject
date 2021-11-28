using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;
using System.Threading.Tasks;

namespace Net_CampMyProject.Services
{
    public class CommentsRepository : RepositoryBase<Comment>, ICommentsRepository
    {
        public CommentsRepository(ApplicationDbContext db) : base(db)
        {

        }

        public async Task<List<Comment>> GetAllCommentsListAsync()
        {
            return await GetAll().Include(c => c.Author).Include(c => c.Film).ToListAsync();
        }

        public override async Task<Comment> GetByIdAsync(int id)
        {
            return await GetAll().AsSplitQuery()
                .Include(c => c.Author)
                .Include(c => c.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}