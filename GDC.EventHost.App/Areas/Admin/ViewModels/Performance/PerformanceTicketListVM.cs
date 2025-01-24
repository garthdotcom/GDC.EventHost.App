using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class PerformanceTicketListVM
    {
        public Guid PerformanceId { get; set; } 
        public string PerformanceTitle { get; set; }
        public string VenueName { get; set; }
        public DateTime PerformanceDate { get; set; }
        public IEnumerable<TicketDetailDto> Tickets { get; set; }
        public string ReturnUrl { get; set; }
    }
}  
