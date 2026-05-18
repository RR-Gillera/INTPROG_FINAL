using System.Collections.Generic;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels.Admin
{
    public class AdminVideosViewModel
    {
        public IEnumerable<Video> Videos { get; set; } = new List<Video>();
        public int TotalCount { get; set; }
    }
}
