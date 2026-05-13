namespace FINALPROJECT.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
        public int OrderIndex { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
