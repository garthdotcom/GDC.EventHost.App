using GDC.EventHost.App.Auth;

namespace GDC.EventHost.App.Areas.Admin.ViewModels.User
{
    public class UserRoleVM
    {
        public UserRoleVM()
        {
            Users = new List<EventHostUser>();
        }

        public string UserId { get; set; }
        public string RoleId { get; set; }
        public List<EventHostUser> Users { get; set; }
    }
}
