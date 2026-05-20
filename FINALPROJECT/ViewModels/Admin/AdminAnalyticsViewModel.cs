using System.Collections.Generic;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels.Admin
{
    public class AdminAnalyticsViewModel
    {
        // Simple lists for Top Contributors and Trending Content
        public IEnumerable<User> TopContributors { get; set; } = new List<User>();
        public IEnumerable<Video> TrendingVideos { get; set; } = new List<Video>();

        // Community Growth chart (monthly)
        public List<string> GrowthLabels { get; set; } = new();
        public List<int> MonthlyUsers { get; set; } = new();
        public List<int> MonthlyRecipes { get; set; } = new();

        // Difficulty distribution
        public int EasyCount { get; set; }
        public int MediumCount { get; set; }
        public int AdvancedCount { get; set; }

        // Category breakdown
        public List<string> CategoryLabels { get; set; } = new();
        public List<int> CategoryCounts { get; set; } = new();
    }
}
