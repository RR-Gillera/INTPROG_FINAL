using FINALPROJECT.Models;
using FINALPROJECT.Repositories;
using FINALPROJECT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FINALPROJECT.Controllers
{
    [Route("collections")]
    [Authorize]
    public class CollectionController : Controller
    {
        private readonly ICollectionRepository _collections;
        private readonly UserManager<User> _userManager;

        public CollectionController(ICollectionRepository collections, UserManager<User> userManager)
        {
            _collections = collections;
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult Index(string tab = "Trending")
        {
            var all = _collections.GetAll().ToList();
            var vm = new CollectionViewModel
            {
                Collections = all,
                Featured    = all.Take(2).ToList(),
                ActiveTab   = tab,
                TotalCount  = _collections.Count()
            };
            return View(vm);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string name, string description)
        {
            var userId = _userManager.GetUserId(User)!;
            _collections.Add(new Collection
            {
                Name        = name,
                Description = description,
                OwnerId     = userId,
                CreatedAt   = DateTime.UtcNow
            });
            return RedirectToAction(nameof(Index));
        }
    }
}
