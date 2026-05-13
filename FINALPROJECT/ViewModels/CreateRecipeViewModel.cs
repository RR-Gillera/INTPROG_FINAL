using System.ComponentModel.DataAnnotations;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels
{
    public class CreateRecipeViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        public IFormFile? HeroImage { get; set; }

        [Range(1, 300)]
        public int PrepTime { get; set; } = 15;

        [Range(1, 600)]
        public int CookTime { get; set; } = 30;

        [Range(1, 100)]
        public int Servings { get; set; } = 4;

        public Difficulty Difficulty { get; set; } = Difficulty.Easy;
        public RecipeCategory Category { get; set; } = RecipeCategory.MainDishes;

        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsGlutenFree { get; set; }

        [Range(0, 9999)]
        public int CaloriesPerServing { get; set; }
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int Carbs { get; set; }

        public List<string> Ingredients { get; set; } = new() { "" };
        public List<StepInput> Steps { get; set; } = new() { new StepInput() };
    }

    public class StepInput
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
