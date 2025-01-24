using GDC.EventHost.App.Auth;
using GDC.EventHost.App.Utilities;

namespace GDC.EventHost.App.Areas.Admin.ViewModels.User
{
    public class UserListVM
    {
        public string SearchQuery { get; set; }
        public PaginatedList<EventHostUser> UserList { get; set; }
    }
} 
