using GDC.EventHost.Shared.Event;
using GDC.EventHost.App.Utilities;

namespace GDC.EventHost.App.ViewModels.Events
{
    public class EventListVM
    { 
        public string SearchQuery { get; set; }

        public PaginatedList<EventDetailDto> EventList { get; set; }

        public List<string> SeriesNames { get; set; }

        public List<string> VenueNames { get; set; }

        public List<string> MonthNames { get; set; }
    }
} 