namespace FINALPROJECT.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public User? Author { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int HelpfulCount { get; set; }
    }
}
