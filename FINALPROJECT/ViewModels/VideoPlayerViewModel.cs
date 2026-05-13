using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels
{
    public class VideoPlayerViewModel
    {
        public Video Video { get; set; } = null!;
        public Video? NextVideo { get; set; }
        public string ActiveTab { get; set; } = "recipe";
    }
}
