using FINALPROJECT.Data;
using FINALPROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace FINALPROJECT.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly FlavorlyDbContext _db;
        public RecipeRepository(FlavorlyDbContext db) => _db = db;

        public IEnumerable<Recipe> GetAll() =>
            _db.Recipes
               .Include(r => r.Author)
               .Include(r => r.Reviews)
               .Include(r => r.Ingredients)
               .Include(r => r.Steps)
               .OrderByDescending(r => r.CreatedAt)
               .ToList();

        public Recipe? GetById(int id) =>
            _db.Recipes
               .Include(r => r.Author)
               .Include(r => r.Ingredients.OrderBy(i => i.OrderIndex))
               .Include(r => r.Steps.OrderBy(s => s.StepNumber))
               .Include(r => r.Reviews)
                   .ThenInclude(rv => rv.Author)
               .FirstOrDefault(r => r.Id == id);

        public IEnumerable<Recipe> GetFiltered(
            RecipeCategory? category,
            Difficulty? difficulty,
            int? maxTime,
            bool? isVeg,
            bool? isVegan,
            bool? isGF,
            string? sortBy)
        {
            var q = _db.Recipes
                        .Include(r => r.Author)
                        .Include(r => r.Reviews)
                        .AsQueryable();

            if (category.HasValue)
                q = q.Where(r => r.Category == category.Value);
            if (difficulty.HasValue)
                q = q.Where(r => r.Difficulty == difficulty.Value);
            if (maxTime.HasValue)
                q = q.Where(r => (r.PrepTime + r.CookTime) <= maxTime.Value);
            if (isVeg == true)
                q = q.Where(r => r.IsVegetarian);
            if (isVegan == true)
                q = q.Where(r => r.IsVegan);
            if (isGF == true)
                q = q.Where(r => r.IsGlutenFree);

            q = sortBy switch
            {
                "oldest"  => q.OrderBy(r => r.CreatedAt),
                "rating"  => q.OrderByDescending(r => r.Reviews.Average(rv => (double?)rv.Rating) ?? 0),
                "time"    => q.OrderBy(r => r.PrepTime + r.CookTime),
                _         => q.OrderByDescending(r => r.CreatedAt)
            };

            return q.ToList();
        }

        public IEnumerable<Recipe> GetByAuthor(string authorId) =>
            _db.Recipes
               .Include(r => r.Author)
               .Include(r => r.Reviews)
               .Where(r => r.AuthorId == authorId)
               .OrderByDescending(r => r.CreatedAt)
               .ToList();

        public IEnumerable<Recipe> GetTrending(int count = 6) =>
            _db.Recipes
               .Include(r => r.Author)
               .Include(r => r.Reviews)
               .OrderByDescending(r => r.Reviews.Count)
               .Take(count)
               .ToList();

        public void Add(Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            _db.SaveChanges();
        }

        public void Update(Recipe recipe)
        {
            _db.Recipes.Update(recipe);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var r = _db.Recipes.Find(id);
            if (r != null) { _db.Recipes.Remove(r); _db.SaveChanges(); }
        }

        public int Count() => _db.Recipes.Count();
    }
}
