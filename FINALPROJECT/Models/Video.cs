namespace FINALPROJECT.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = "/images/default-thumb.jpg";
        public string VideoUrl { get; set; } = string.Empty;
        public string Duration { get; set; } = "0:00";
        public int ViewCount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public User? Author { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Ingredients { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
    }
}
