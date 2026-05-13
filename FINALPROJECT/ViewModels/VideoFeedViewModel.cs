using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels
{
    public class VideoFeedViewModel
    {
        public Video? FeaturedVideo { get; set; }
        public IEnumerable<Video> Videos { get; set; } = new List<Video>();
        public string ActiveCategory { get; set; } = "All";
        public IEnumerable<string> Categories { get; set; } = new List<string>();
    }
}
