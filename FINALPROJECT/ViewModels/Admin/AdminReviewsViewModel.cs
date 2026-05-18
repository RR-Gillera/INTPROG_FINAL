using System.Collections.Generic;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels.Admin
{
    public class AdminReviewsViewModel
    {
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        public IEnumerable<Report> Reports { get; set; } = new List<Report>();
        public int PendingReportsCount { get; set; }
        public int NewReviewsCount { get; set; }
    }
}
