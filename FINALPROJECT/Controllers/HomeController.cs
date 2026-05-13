using FINALPROJECT.Models;
using FINALPROJECT.Repositories;
using FINALPROJECT.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FINALPROJECT.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRecipeRepository _recipes;
        private readonly ITipRepository _tips;
        private readonly IVideoRepository _videos;

        public HomeController(IRecipeRepository recipes, ITipRepository tips, IVideoRepository videos)
        {
            _recipes = recipes;
            _tips = tips;
            _videos = videos;
        }

        public IActionResult Index(string tab = "Trending")
        {
            IEnumerable<Recipe> featured;
            featured = tab switch
            {
                "Vegetarian"  => _recipes.GetFiltered(null, null, null, true, null, null, "newest").Take(6),
                "Beginner"    => _recipes.GetFiltered(null, Difficulty.Easy, null, null, null, null, "newest").Take(6),
                "Quick Meals" => _recipes.GetFiltered(null, null, 30, null, null, null, "newest").Take(6),
                _             => _recipes.GetTrending(6)
            };

            var vm = new HomeViewModel
            {
                TrendingRecipes = featured,
                RecentTips      = _tips.GetAll().Take(4),
                ActiveTab       = tab,
                TotalRecipes    = _recipes.Count(),
                TotalVideos     = _videos.GetAll().Count()
            };
            return View(vm);
        }

        [Route("search")]
        public IActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return RedirectToAction(nameof(Index));

            var results = _recipes.GetAll()
                .Where(r => r.Title.Contains(q, StringComparison.OrdinalIgnoreCase)
                         || r.Description.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewBag.Query = q;
            return View("SearchResults", results);
        }

        [Route("error/404")]
        public IActionResult NotFoundPage() => View("NotFound");
    }
}
