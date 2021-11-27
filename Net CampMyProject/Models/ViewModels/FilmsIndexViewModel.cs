using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Net_CampMyProject.Controllers;
using Net_CampMyProject.Data.Models;

namespace Net_CampMyProject.Models.ViewModels
{
    public class FilmsIndexViewModel
    {
        public ICollection<Film> Films { get; set; }
        public PaginationPageViewModel PaginationPageViewModel { get; set; }
        public FilmsFilterType Filter { get; set; }
        public SortOrder SortOrder { get; set; }
        public string SortBy { get; set; }
    }
}
