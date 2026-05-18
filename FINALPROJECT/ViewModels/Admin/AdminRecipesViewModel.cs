using System.Collections.Generic;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels.Admin
{
    public class AdminRecipesViewModel
    {
        public IEnumerable<Recipe> Recipes { get; set; } = new List<Recipe>();
        public int TotalCount { get; set; }
    }
}
