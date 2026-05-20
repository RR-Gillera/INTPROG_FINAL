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
            // Compute daily new-user and new-recipe counts for the last 14 days
            var today = DateTime.UtcNow.Date;
            var chartLabels = new List<string>();
            var newUsersPerDay = new List<int>();
            var newRecipesPerDay = new List<int>();

            for (int i = 13; i >= 0; i--)
            {
                var day = today.AddDays(-i);
                var nextDay = day.AddDays(1);
                chartLabels.Add(day.ToString("MMM dd"));
                newUsersPerDay.Add(await _userManager.Users.CountAsync(u => u.CreatedAt >= day && u.CreatedAt < nextDay));
                newRecipesPerDay.Add(await _db.Recipes.CountAsync(r => r.CreatedAt >= day && r.CreatedAt < nextDay));
            }

            var vm = new FINALPROJECT.ViewModels.Admin.AdminDashboardViewModel
            {
                TotalRecipes = await _db.Recipes.CountAsync(),
                TotalVideos = await _db.Videos.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                PendingReportsCount = await _db.Reports.CountAsync(r => r.Status == "PENDING"),
                ChartLabels = chartLabels,
                NewUsersPerDay = newUsersPerDay,
                NewRecipesPerDay = newRecipesPerDay,
                RecentUsers = await _userManager.Users.OrderByDescending(u => u.CreatedAt).Take(3).ToListAsync(),
                RecentRecipeActivity = await _db.Recipes.Include(r => r.Author).OrderByDescending(r => r.CreatedAt).Take(3).ToListAsync(),
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
            var video = await _db.Videos.FindAsync(id);
            if (video != null)
            {
                video.IsFeatured = !video.IsFeatured;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Videos));
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

        [HttpPost("settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSettings(FINALPROJECT.ViewModels.Admin.AdminSettingsViewModel vm)
        {
            var settings = await _db.SystemSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new SystemSetting();
                _db.SystemSettings.Add(settings);
            }

            settings.SiteName = vm.Settings.SiteName;
            settings.Tagline = vm.Settings.Tagline;
            settings.MaintenanceMode = vm.Settings.MaintenanceMode;
            settings.AutoFlaggingKeywords = vm.Settings.AutoFlaggingKeywords;
            settings.AutoHideReportsThreshold = vm.Settings.AutoHideReportsThreshold;
            settings.RequireManualApproval = vm.Settings.RequireManualApproval;
            settings.SmtpHost = vm.Settings.SmtpHost;
            settings.SmtpPort = vm.Settings.SmtpPort;
            settings.SmtpUsername = vm.Settings.SmtpUsername;

            if (!string.IsNullOrEmpty(vm.Settings.SmtpPassword))
            {
                settings.SmtpPassword = vm.Settings.SmtpPassword;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "System settings updated successfully!";
            return RedirectToAction(nameof(Settings));
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

        [HttpPost("toggle-ban/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBan(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var isLockedOut = await _userManager.IsLockedOutAsync(user);
                if (isLockedOut)
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    TempData["SuccessMessage"] = $"User {user.DisplayName} has been successfully unbanned.";
                }
                else
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                    TempData["SuccessMessage"] = $"User {user.DisplayName} has been successfully suspended (banned).";
                }
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost("promote/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                    TempData["SuccessMessage"] = $"Demoted {user.DisplayName} to normal user status.";
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    TempData["SuccessMessage"] = $"Promoted {user.DisplayName} to Admin status.";
                }
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost("delete-user/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var currentUserId = _userManager.GetUserId(User);
                if (user.Id == currentUserId)
                {
                    TempData["ErrorMessage"] = "You cannot delete your own active administrator account!";
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                    TempData["SuccessMessage"] = $"User {user.DisplayName} was permanently deleted.";
                }
            }
            return RedirectToAction(nameof(Users));
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

        [HttpPost("dismiss-report/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DismissReport(int id)
        {
            var report = await _db.Reports.FindAsync(id);
            if (report != null)
            {
                report.Status = "DISMISSED";
                report.ResolvedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Report was successfully dismissed.";
            }
            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost("resolve-report/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveReport(int id)
        {
            var report = await _db.Reports.FindAsync(id);
            if (report != null)
            {
                report.Status = "RESOLVED";
                report.ResolvedAt = DateTime.UtcNow;

                if (report.ContentType.Equals("Recipe", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(report.ContentId, out int recipeId))
                    {
                        var recipe = await _db.Recipes.FindAsync(recipeId);
                        if (recipe != null)
                        {
                            _db.Recipes.Remove(recipe);
                        }
                    }
                }
                else if (report.ContentType.Equals("Video", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(report.ContentId, out int videoId))
                    {
                        var video = await _db.Videos.FindAsync(videoId);
                        if (video != null)
                        {
                            _db.Videos.Remove(video);
                        }
                    }
                }
                else if (report.ContentType.Equals("Review", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(report.ContentId, out int reviewId))
                    {
                        var review = await _db.Reviews.FindAsync(reviewId);
                        if (review != null)
                        {
                            _db.Reviews.Remove(review);
                        }
                    }
                }
                else if (report.ContentType.Equals("User", StringComparison.OrdinalIgnoreCase))
                {
                    var offendingUser = await _userManager.FindByIdAsync(report.ContentId);
                    if (offendingUser != null)
                    {
                        await _userManager.SetLockoutEnabledAsync(offendingUser, true);
                        await _userManager.SetLockoutEndDateAsync(offendingUser, DateTimeOffset.MaxValue);
                    }
                }

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Report has been successfully resolved and offending content moderated.";
            }
            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost("approve-review/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReview(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review != null)
            {
                var reports = await _db.Reports.Where(r => r.ContentType == "Review" && r.ContentId == id.ToString()).ToListAsync();
                foreach (var report in reports)
                {
                    report.Status = "DISMISSED";
                    report.ResolvedAt = DateTime.UtcNow;
                }
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Review was approved and associated reports were cleared.";
            }
            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost("delete-review/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review != null)
            {
                _db.Reviews.Remove(review);

                var reports = await _db.Reports.Where(r => r.ContentType == "Review" && r.ContentId == id.ToString()).ToListAsync();
                foreach (var report in reports)
                {
                    report.Status = "RESOLVED";
                    report.ResolvedAt = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Review was permanently deleted and associated reports resolved.";
            }
            return RedirectToAction(nameof(Reviews));
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics()
        {
            // Monthly growth for the last 6 months
            var growthLabels = new List<string>();
            var monthlyUsers = new List<int>();
            var monthlyRecipes = new List<int>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1);
                growthLabels.Add(monthStart.ToString("MMM"));
                monthlyUsers.Add(await _userManager.Users.CountAsync(u => u.CreatedAt >= monthStart && u.CreatedAt < monthEnd));
                monthlyRecipes.Add(await _db.Recipes.CountAsync(r => r.CreatedAt >= monthStart && r.CreatedAt < monthEnd));
            }

            // Difficulty distribution
            var easyCount = await _db.Recipes.CountAsync(r => r.Difficulty == Difficulty.Easy);
            var mediumCount = await _db.Recipes.CountAsync(r => r.Difficulty == Difficulty.Medium);
            var advancedCount = await _db.Recipes.CountAsync(r => r.Difficulty == Difficulty.Advanced);

            // Category breakdown
            var categories = await _db.Recipes
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            var vm = new FINALPROJECT.ViewModels.Admin.AdminAnalyticsViewModel
            {
                TopContributors = await _userManager.Users.Include(u => u.Recipes).OrderByDescending(u => u.Recipes.Count).Take(5).ToListAsync(),
                TrendingVideos = await _db.Videos.Include(v => v.Author).OrderByDescending(v => v.ViewCount).Take(5).ToListAsync(),
                GrowthLabels = growthLabels,
                MonthlyUsers = monthlyUsers,
                MonthlyRecipes = monthlyRecipes,
                EasyCount = easyCount,
                MediumCount = mediumCount,
                AdvancedCount = advancedCount,
                CategoryLabels = categories.Select(c => c.Category.ToString()).ToList(),
                CategoryCounts = categories.Select(c => c.Count).ToList()
            };
            return View(vm);
        }
    }
}
