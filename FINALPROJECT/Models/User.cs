using Microsoft.AspNetCore.Identity;

namespace FINALPROJECT.Models
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = "/images/default-avatar.png";
        public string CoverImageUrl { get; set; } = "/images/default-cover.jpg";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<Video> Videos { get; set; } = new List<Video>();
        public ICollection<Tip> Tips { get; set; } = new List<Tip>();
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
    }
}
