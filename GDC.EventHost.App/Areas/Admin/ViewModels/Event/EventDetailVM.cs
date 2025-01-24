using GDC.EventHost.Shared.Event;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class EventDetailVM 
    {
        public EventDetailDto Event { get; set; }

        public string ReturnUrl { get; set; }
    }
}
