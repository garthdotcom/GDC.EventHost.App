using GDC.EventHost.Shared.Ticket;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class TicketEditVM
    {
        public TicketForUpdateDto Ticket { get; set; }

        public List<SelectListItem> TicketStatusList { get; set; }

        public string ReturnUrl { get; set; }
    }
}
