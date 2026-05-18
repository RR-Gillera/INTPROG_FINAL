using System.Collections.Generic;
using FINALPROJECT.Models;

namespace FINALPROJECT.ViewModels.Admin
{
    public class AdminUsersViewModel
    {
        public IEnumerable<User> Users { get; set; } = new List<User>();
        public int TotalCount { get; set; }
    }
}
