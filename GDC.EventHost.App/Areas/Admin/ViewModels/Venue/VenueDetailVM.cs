using GDC.EventHost.Shared.Venue;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class VenueDetailVM
    {
        public VenueDetailDto Venue { get; set; }

        public string ReturnUrl { get; set; }
    }
}
