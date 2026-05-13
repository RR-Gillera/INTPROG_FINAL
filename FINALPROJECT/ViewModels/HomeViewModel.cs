using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Recipe> TrendingRecipes { get; set; } = new List<Recipe>();
        public IEnumerable<Tip> RecentTips { get; set; } = new List<Tip>();
        public string ActiveTab { get; set; } = "Trending";
        public int TotalRecipes { get; set; }
        public int TotalVideos { get; set; }
    }
}
