using FINALPROJECT.Data;
using FINALPROJECT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FINALPROJECT.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly FlavorlyDbContext _db;
        private readonly UserManager<User> _userManager;

        public AdminController(FlavorlyDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Dashboard()
        {
            var vm = new FINALPROJECT.ViewModels.Admin.AdminDashboardViewModel
            {
                TotalRecipes = await _db.Recipes.CountAsync(),
                TotalVideos = await _db.Videos.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                RecentRecipes = await _db.Recipes
                    .Include(r => r.Author)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .ToListAsync()
            };

            return View(vm);
        }

        [HttpPost("delete-recipe/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _db.Recipes.Remove(recipe);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost("toggle-featured/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe != null)
            {
                // Note: IsFeatured doesn't exist on Recipe model yet, let's check.
                // It's on Video though.
                // For now, let's just do deletion.
            }
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet("settings")]
        public async Task<IActionResult> Settings()
        {
            var settings = await _db.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            var vm = new FINALPROJECT.ViewModels.Admin.AdminSettingsViewModel
            {
                Settings = settings
            };
            return View(vm);
        }

        [HttpGet("recipes")]
        public async Task<IActionResult> Recipes()
        {
            var recipes = await _db.Recipes.Include(r => r.Author).OrderByDescending(r => r.CreatedAt).ToListAsync();
            var vm = new FINALPROJECT.ViewModels.Admin.AdminRecipesViewModel
            {
                Recipes = recipes,
                TotalCount = recipes.Count
            };
            return View(vm);
        }

        [HttpGet("videos")]
        public async Task<IActionResult> Videos()
        {
            var videos = await _db.Videos.Include(v => v.Author).OrderByDescending(v => v.CreatedAt).ToListAsync();
            var vm = new FINALPROJECT.ViewModels.Admin.AdminVideosViewModel
            {
                Videos = videos,
                TotalCount = videos.Count
            };
            return View(vm);
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.Include(u => u.Recipes).OrderByDescending(u => u.Id).ToListAsync();
            var vm = new FINALPROJECT.ViewModels.Admin.AdminUsersViewModel
            {
                Users = users,
                TotalCount = users.Count
            };
            return View(vm);
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> Reviews()
        {
            var reviews = await _db.Reviews.Include(r => r.Author).Include(r => r.Recipe).OrderByDescending(r => r.CreatedAt).Take(20).ToListAsync();
            var reports = await _db.Reports.Include(r => r.ReportedBy).OrderByDescending(r => r.CreatedAt).Take(20).ToListAsync();
            var vm = new FINALPROJECT.ViewModels.Admin.AdminReviewsViewModel
            {
                Reviews = reviews,
                Reports = reports,
                PendingReportsCount = await _db.Reports.CountAsync(r => r.Status == "PENDING"),
                NewReviewsCount = await _db.Reviews.CountAsync(r => r.CreatedAt >= DateTime.UtcNow.AddDays(-7))
            };
            return View(vm);
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics()
        {
            var vm = new FINALPROJECT.ViewModels.Admin.AdminAnalyticsViewModel
            {
                TopContributors = await _userManager.Users.Include(u => u.Recipes).OrderByDescending(u => u.Recipes.Count).Take(5).ToListAsync(),
                TrendingVideos = await _db.Videos.Include(v => v.Author).OrderByDescending(v => v.ViewCount).Take(5).ToListAsync()
            };
            return View(vm);
        }
    }
}
