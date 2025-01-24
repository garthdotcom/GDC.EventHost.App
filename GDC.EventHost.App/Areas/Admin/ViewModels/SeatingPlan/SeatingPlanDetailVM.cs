using GDC.EventHost.Shared.SeatingPlan;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeatingPlanDetailVM
    {
        public SeatingPlanDetailDto SeatingPlanDetail { get; set; } 

        public string ReturnUrl { get; set; }

        public Guid? PerformanceId { get; set; } 
    }
}
