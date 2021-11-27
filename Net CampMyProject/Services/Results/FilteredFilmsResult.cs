using System.Collections.Generic;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Services.Results
{
    public class FilteredFilmsResult
    {
        public List<Film> Films { get; set; }
        public int Count { get; set; }
    }
}