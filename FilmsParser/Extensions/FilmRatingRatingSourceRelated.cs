using Net_CampMyProject.Data.Models;

namespace FilmsParser.Extensions
{
    public class FilmRatingRatingSourceRelated
    {
        public string RatingValue { get; set; }

        public  FilmRatingSource filmRatingSource { get;set ;}
    }
}
