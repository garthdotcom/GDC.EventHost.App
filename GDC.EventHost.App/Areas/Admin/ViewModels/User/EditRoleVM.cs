using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.App.Admin.ViewModels.User
{
    public class EditRoleVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please enter the role name")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
