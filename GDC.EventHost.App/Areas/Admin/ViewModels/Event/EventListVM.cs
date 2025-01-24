using GDC.EventHost.Shared.Event;
using GDC.EventHost.App.Utilities;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class EventListVM
    { 
        public string SearchQuery { get; set; }
        public PaginatedList<EventDetailDto> EventList { get; set; }
    }
}