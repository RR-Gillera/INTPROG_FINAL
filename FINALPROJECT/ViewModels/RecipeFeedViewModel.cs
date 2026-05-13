using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels
{
    public class RecipeFeedViewModel
    {
        public IEnumerable<Recipe> Recipes { get; set; } = new List<Recipe>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // Filter state
        public RecipeCategory? Category { get; set; }
        public Difficulty? Difficulty { get; set; }
        public int? MaxTime { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public string SortBy { get; set; } = "newest";
    }
}
