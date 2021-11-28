using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Net_CampMyProject.Services
{
    public class PersonsRepository : RepositoryBase<Person>, IPersonsRepository
    {
        public PersonsRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<List<Person>> GetAllPersonListAsync()
        {
            return await GetAll().ToListAsync();
        }
    }
}