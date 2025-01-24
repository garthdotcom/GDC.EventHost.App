using GDC.EventHost.Shared.Venue;

namespace GDC.EventHost.App.ViewModels.Venues
{
    public class VenueListVM
    {
        public string SearchQuery { get; set; }
        public IEnumerable<VenueDetailDto> VenueList { get; set; }
    }
}
