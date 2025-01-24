using GDC.EventHost.Shared.Event;

namespace GDC.EventHost.App.ViewModels.Events
{
    public class EventDetailVM 
    {
        public EventDetailDto Event { get; set; }

        public string ReturnUrl { get; set; }
    }
}
