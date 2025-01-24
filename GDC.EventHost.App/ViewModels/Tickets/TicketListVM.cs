using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.App.ViewModels.Tickets
{
    public class TicketListVM
    {
        public Guid PerformanceId { get; set; }
        public string PerformanceTitle { get; set; }
        public string VenueName { get; set; }
        public DateTime EventDate { get; set; }
        public IEnumerable<TicketDetailDto> Tickets { get; set; }
    }
}
