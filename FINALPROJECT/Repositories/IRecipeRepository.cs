using FINALPROJECT.Models;

namespace FINALPROJECT.Repositories
{
    public interface IRecipeRepository
    {
        IEnumerable<Recipe> GetAll();
        Recipe? GetById(int id);
        IEnumerable<Recipe> GetFiltered(
            RecipeCategory? category,
            Difficulty? difficulty,
            int? maxTime,
            bool? isVeg,
            bool? isVegan,
            bool? isGF,
            string? sortBy);
        IEnumerable<Recipe> GetByAuthor(string authorId);
        IEnumerable<Recipe> GetTrending(int count = 6);
        void Add(Recipe recipe);
        void Update(Recipe recipe);
        void Delete(int id);
        int Count();
    }
}
