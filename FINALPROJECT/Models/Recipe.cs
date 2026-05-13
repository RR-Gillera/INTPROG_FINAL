namespace FINALPROJECT.Models
{
    public enum Difficulty { Easy, Medium, Advanced }
    public enum RecipeCategory { BreakfastBrunch, MainDishes, Desserts, SnacksApps }

    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = "/images/default-recipe.jpg";
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public Difficulty Difficulty { get; set; } = Difficulty.Easy;
        public RecipeCategory Category { get; set; } = RecipeCategory.MainDishes;
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }
        public int CaloriesPerServing { get; set; }
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int Carbs { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AuthorId { get; set; } = string.Empty;
        public User? Author { get; set; }
        public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
        public ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0.0;
    }
}
