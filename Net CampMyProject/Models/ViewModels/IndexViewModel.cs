using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Models.ViewModels
{
    public class IndexViewModel
    {
        public ICollection<Film> Films { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
