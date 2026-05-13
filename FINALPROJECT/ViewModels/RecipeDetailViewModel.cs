using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels
{
    public class RecipeDetailViewModel
    {
        public Recipe Recipe { get; set; } = null!;
        public IEnumerable<Recipe> RelatedRecipes { get; set; } = new List<Recipe>();
        public bool IsFollowingAuthor { get; set; }
        public string? CurrentUserId { get; set; }
        public string NewReviewComment { get; set; } = string.Empty;
        public int NewReviewRating { get; set; } = 5;
    }
}
