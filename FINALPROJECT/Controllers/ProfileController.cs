using FINALPROJECT.Data;
using FINALPROJECT.Models;
using FINALPROJECT.Repositories;
using FINALPROJECT.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FINALPROJECT.Controllers
{
    [Route("profile")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRecipeRepository _recipes;
        private readonly IVideoRepository _videos;
        private readonly ITipRepository _tips;
        private readonly ICollectionRepository _collections;
        private readonly FlavorlyDbContext _db;

        public ProfileController(UserManager<User> userManager, IRecipeRepository recipes,
            IVideoRepository videos, ITipRepository tips, ICollectionRepository collections,
            FlavorlyDbContext db)
        {
            _userManager  = userManager;
            _recipes      = recipes;
            _videos       = videos;
            _tips         = tips;
            _collections  = collections;
            _db           = db;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Index(string username, string tab = "Recipes")
        {
            var profileUser = await _userManager.FindByNameAsync(username)
                           ?? await _userManager.Users.FirstOrDefaultAsync(u => u.DisplayName == username);

            if (profileUser == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            var isFollowing   = false;
            if (currentUserId != null && currentUserId != profileUser.Id)
                isFollowing = _db.Follows.Any(f => f.FollowerId == currentUserId && f.FollowingId == profileUser.Id);

            var vm = new UserProfileViewModel
            {
                ProfileUser    = profileUser,
                Recipes        = _recipes.GetByAuthor(profileUser.Id),
                Videos         = _videos.GetAll().Where(v => v.AuthorId == profileUser.Id),
                Tips           = _tips.GetByAuthor(profileUser.Id),
                Collections    = _collections.GetByOwner(profileUser.Id),
                FollowersCount = _db.Follows.Count(f => f.FollowingId == profileUser.Id),
                FollowingCount = _db.Follows.Count(f => f.FollowerId == profileUser.Id),
                IsFollowing    = isFollowing,
                IsOwnProfile   = currentUserId == profileUser.Id,
                ActiveTab      = tab
            };
            return View(vm);
        }

        [HttpPost("{username}/follow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(string username)
        {
            var target    = await _userManager.FindByNameAsync(username);
            var currentId = _userManager.GetUserId(User);
            if (target == null || currentId == null) return NotFound();

            var existing = _db.Follows.FirstOrDefault(f => f.FollowerId == currentId && f.FollowingId == target.Id);
            if (existing == null)
                _db.Follows.Add(new Follow { FollowerId = currentId, FollowingId = target.Id });
            else
                _db.Follows.Remove(existing);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { username });
        }
    }
}
