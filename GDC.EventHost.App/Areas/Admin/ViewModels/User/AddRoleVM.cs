using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.App.Admin.ViewModels.User
{
    public class AddRoleVM
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
