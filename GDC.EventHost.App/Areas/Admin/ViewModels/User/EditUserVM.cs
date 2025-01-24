using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.App.Areas.Admin.ViewModels.User
{
    public class EditUserVM
    {
        public string Id { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Please enter the user name")]
        public string UserName { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Please enter the user email")]
        public string Email { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please enter the user's name")]
        public string FullName { get; set; }

        public Guid? MemberId { get; set; }

        public List<string> UserClaims { get; set; }
    }
}
