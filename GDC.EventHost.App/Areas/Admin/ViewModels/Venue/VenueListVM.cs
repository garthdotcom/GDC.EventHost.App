using GDC.EventHost.Shared.Venue;
using GDC.EventHost.App.Utilities;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class VenueListVM
    {
        public string SearchQuery { get; set; }
        public PaginatedList<VenueDetailDto> VenueList { get; set; }
    }
}
