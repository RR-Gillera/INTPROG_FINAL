namespace FINALPROJECT.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = "/images/default-collection.jpg";
        public string OwnerId { get; set; } = string.Empty;
        public User? Owner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<CollectionRecipe> CollectionRecipes { get; set; } = new List<CollectionRecipe>();
    }
}
