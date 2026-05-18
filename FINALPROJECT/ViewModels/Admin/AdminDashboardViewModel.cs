using System.Collections.Generic;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalRecipes { get; set; }
        public int TotalVideos { get; set; }
        public int TotalUsers { get; set; }
        
        public IEnumerable<Recipe> RecentRecipes { get; set; } = new List<Recipe>();
    }
}
