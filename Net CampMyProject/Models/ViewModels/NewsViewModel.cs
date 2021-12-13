using System;

namespace Net_CampMyProject.Models.ViewModels
{
    public class NewsViewModel 
    {
        public string Title { get; set; }

        public  string Description { get; set; }

        public  DateTime Date { get; set; }

        public string FullText { get; set; }

        public string ImgLink { get; set; }

        public string NewsLink { get; set; }
    }
}
