using Microsoft.AspNetCore.Identity;

namespace GDC.EventHost.App.Auth
{
    public class EventHostUser : IdentityUser 
    {
        public DateTime? BirthDate { get; set; }
        public Guid? MemberId { get; set; }
    }
}
