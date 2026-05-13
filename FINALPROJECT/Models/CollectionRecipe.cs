namespace FINALPROJECT.Models
{
    public class CollectionRecipe
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public Collection? Collection { get; set; }
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
