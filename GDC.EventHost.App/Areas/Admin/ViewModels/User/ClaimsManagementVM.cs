namespace GDC.EventHost.App.Areas.Admin.ViewModels.User
{
    public class ClaimsManagementVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; } 
        public string ClaimId { get; set; }
        public List<UserClaimVM> UserClaimVMs { get; set; }
    }
}