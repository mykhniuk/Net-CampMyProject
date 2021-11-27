using System;

namespace Net_CampMyProject.Models.ViewModels
{
     public class PaginationPageViewModel
    {
        public int PageNumber { get; }
        public int TotalPages { get; }

        public PaginationPageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage => (PageNumber > 1);

        public bool HasNextPage => (PageNumber < TotalPages);
    }
}
