using GDC.EventHost.Shared.SeatingPlan;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeatingPlanAddVM
    {
        public bool PositionsCreated { get; set; }

        public SeatingPlanForCreateDto SeatingPlan { get; set; }

        public List<SelectListItem> VenueList { get; set; }

        public Guid? PerformanceId { get; set; }
    }
}
