using System.Collections.Generic;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalRecipes { get; set; }
        public int TotalVideos { get; set; }
        public int TotalUsers { get; set; }
        public int PendingReportsCount { get; set; }

        // Chart data: daily counts for the last 14 days
        public List<string> ChartLabels { get; set; } = new();
        public List<int> NewUsersPerDay { get; set; } = new();
        public List<int> NewRecipesPerDay { get; set; } = new();

        // Recent activity items
        public IEnumerable<User> RecentUsers { get; set; } = new List<User>();
        public IEnumerable<Recipe> RecentRecipeActivity { get; set; } = new List<Recipe>();

        public IEnumerable<Recipe> RecentRecipes { get; set; } = new List<Recipe>();
    }
}
