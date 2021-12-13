using Microsoft.AspNetCore.Authorization;
using Net_CampMyProject.Data.Models;
using Net_CampMyProject.Models;
using Net_CampMyProject.Services.Interfaces;

namespace Net_CampMyProject.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class PersonsController : BaseController<Person>
    {
        public PersonsController(IPersonsRepository personRepository) : base(personRepository)
        {
        }
    }
}
