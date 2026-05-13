using FINALPROJECT.Data;
using FINALPROJECT.Models;
using FINALPROJECT.Repositories;
using FINALPROJECT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FINALPROJECT.Controllers
{
    [Route("recipes")]
    public class RecipeController : Controller
    {
        private readonly IRecipeRepository _recipes;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly FlavorlyDbContext _db;

        public RecipeController(IRecipeRepository recipes, UserManager<User> userManager,
            IWebHostEnvironment env, FlavorlyDbContext db)
        {
            _recipes = recipes;
            _userManager = userManager;
            _env = env;
            _db = db;
        }

        [HttpGet("")]
        public IActionResult Feed(RecipeCategory? category, Difficulty? difficulty,
            int? maxTime, bool? isVeg, bool? isVegan, bool? isGF, string sortBy = "newest", int page = 1)
        {
            const int pageSize = 9;
            var filtered = _recipes.GetFiltered(category, difficulty, maxTime, isVeg, isVegan, isGF, sortBy).ToList();
            var paged    = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var vm = new RecipeFeedViewModel
            {
                Recipes      = paged,
                TotalCount   = filtered.Count,
                CurrentPage  = page,
                PageSize     = pageSize,
                Category     = category,
                Difficulty   = difficulty,
                MaxTime      = maxTime,
                IsVegetarian = isVeg ?? false,
                IsVegan      = isVegan ?? false,
                IsGlutenFree = isGF ?? false,
                SortBy       = sortBy
            };
            return View("Feed", vm);
        }

        [HttpGet("{id:int}")]
        public IActionResult Detail(int id)
        {
            var recipe = _recipes.GetById(id);
            if (recipe == null) return NotFound();

            var related = _recipes.GetAll()
                .Where(r => r.Category == recipe.Category && r.Id != id)
                .Take(2).ToList();

            var vm = new RecipeDetailViewModel
            {
                Recipe         = recipe,
                RelatedRecipes = related,
                CurrentUserId  = _userManager.GetUserId(User)
            };
            return View("Detail", vm);
        }

        [HttpGet("create")]
        [Authorize]
        public IActionResult Create() => View(new CreateRecipeViewModel());

        [HttpPost("create")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRecipeViewModel model)
        {
            // Remove empty ingredient/step entries
            model.Ingredients = model.Ingredients.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            model.Steps       = model.Steps.Where(s => !string.IsNullOrWhiteSpace(s.Title)).ToList();

            if (!model.Ingredients.Any())
                ModelState.AddModelError("Ingredients", "Add at least one ingredient.");
            if (!model.Steps.Any())
                ModelState.AddModelError("Steps", "Add at least one step.");

            if (!ModelState.IsValid) return View(model);

            string heroUrl = "/images/default-recipe.jpg";
            if (model.HeroImage != null && model.HeroImage.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.HeroImage.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await model.HeroImage.CopyToAsync(stream);
                heroUrl = $"/uploads/{fileName}";
            }

            var recipe = new Recipe
            {
                Title              = model.Title,
                Description        = model.Description,
                HeroImageUrl       = heroUrl,
                PrepTime           = model.PrepTime,
                CookTime           = model.CookTime,
                Servings           = model.Servings,
                Difficulty         = model.Difficulty,
                Category           = model.Category,
                IsVegetarian       = model.IsVegetarian,
                IsVegan            = model.IsVegan,
                IsGlutenFree       = model.IsGlutenFree,
                CaloriesPerServing = model.CaloriesPerServing,
                Protein            = model.Protein,
                Fat                = model.Fat,
                Carbs              = model.Carbs,
                AuthorId           = _userManager.GetUserId(User)!,
                CreatedAt          = DateTime.UtcNow,
                Ingredients        = model.Ingredients.Select((txt, idx) => new RecipeIngredient
                                         { OrderIndex = idx, Text = txt }).ToList(),
                Steps              = model.Steps.Select((s, idx) => new RecipeStep
                                         { StepNumber = idx + 1, Title = s.Title, Description = s.Description }).ToList()
            };

            _recipes.Add(recipe);
            return RedirectToAction("Detail", new { id = recipe.Id });
        }

        [HttpPost("{id:int}/review")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(int id, int rating, string comment)
        {
            var userId = _userManager.GetUserId(User)!;
            _db.Reviews.Add(new Review
            {
                RecipeId  = id,
                AuthorId  = userId,
                Rating    = Math.Clamp(rating, 1, 5),
                Comment   = comment ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            });
            _db.SaveChanges();
            return RedirectToAction("Detail", new { id });
        }
    }
}
