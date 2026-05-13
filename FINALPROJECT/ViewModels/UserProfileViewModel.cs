using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels
{
    public class UserProfileViewModel
    {
        public User ProfileUser { get; set; } = null!;
        public IEnumerable<Recipe> Recipes { get; set; } = new List<Recipe>();
        public IEnumerable<Video> Videos { get; set; } = new List<Video>();
        public IEnumerable<Tip> Tips { get; set; } = new List<Tip>();
        public IEnumerable<Collection> Collections { get; set; } = new List<Collection>();
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsOwnProfile { get; set; }
        public string ActiveTab { get; set; } = "Recipes";
    }
}
