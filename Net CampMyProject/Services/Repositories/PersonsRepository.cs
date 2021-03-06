using Net_CampMyProject.Data;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Services
{
    public class PersonsRepository : RepositoryBase<Person>, IPersonsRepository
    {
        public PersonsRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}