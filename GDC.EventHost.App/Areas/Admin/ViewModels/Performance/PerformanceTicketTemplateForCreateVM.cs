using GDC.EventHost.Shared.Ticket;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class PerformanceTicketTemplateForCreateVM
    {
        public PerformanceTicketForCreateDto PerformanceTicketForCreate { get; set; }
        public string PerformanceTitle { get; set; }
        public string SeatingPlanName { get; set; }
        public string ReturnUrl { get; set; } 
    }
} 
