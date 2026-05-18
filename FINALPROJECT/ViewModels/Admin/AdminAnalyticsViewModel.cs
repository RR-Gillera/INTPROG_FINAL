using System.Collections.Generic;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels.Admin
{
    public class AdminAnalyticsViewModel
    {
        // Simple lists for Top Contributors and Trending Content
        public IEnumerable<User> TopContributors { get; set; } = new List<User>();
        public IEnumerable<Video> TrendingVideos { get; set; } = new List<Video>();
    }
}
