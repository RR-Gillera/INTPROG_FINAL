using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels
{
    public class CollectionViewModel
    {
        public IEnumerable<Collection> Collections { get; set; } = new List<Collection>();
        public IEnumerable<Collection> Featured { get; set; } = new List<Collection>();
        public string ActiveTab { get; set; } = "Trending";
        public int TotalCount { get; set; }
        public string? NewCollectionName { get; set; }
        public string? NewCollectionDescription { get; set; }
    }
}
