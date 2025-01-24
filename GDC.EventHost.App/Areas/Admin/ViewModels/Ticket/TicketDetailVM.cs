using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class TicketDetailVM
    {
        public TicketDetailDto Ticket { get; set; }
        public string ReturnUrl { get; set; }
    }
}
