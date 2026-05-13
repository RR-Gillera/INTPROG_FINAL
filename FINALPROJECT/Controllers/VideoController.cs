using FINALPROJECT.Repositories;
using FINALPROJECT.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FINALPROJECT.Controllers
{
    [Route("videos")]
    public class VideoController : Controller
    {
        private readonly IVideoRepository _videos;
        public VideoController(IVideoRepository videos) => _videos = videos;

        private static readonly List<string> _categories = new()
        {
            "All", "Baking", "Vegetarian", "Quick Meals",
            "Desserts", "Italian Cuisine", "Cocktails", "Fermentation"
        };

        [HttpGet("")]
        public IActionResult Feed(string category = "All")
        {
            var vm = new VideoFeedViewModel
            {
                FeaturedVideo  = _videos.GetFeatured(),
                Videos         = _videos.GetByCategory(category),
                ActiveCategory = category,
                Categories     = _categories
            };
            return View("Feed", vm);
        }

        [HttpGet("{id:int}")]
        public IActionResult Player(int id)
        {
            var video = _videos.GetById(id);
            if (video == null) return NotFound();

            var vm = new VideoPlayerViewModel
            {
                Video     = video,
                NextVideo = _videos.GetNext(id)
            };
            return View("Player", vm);
        }
    }
}
