using GDC.EventHost.Shared.Performance;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class AddSeatingPlanToPerformanceVM
    {
        public PerformanceDetailDto PerformanceDetail { get; set; }

        public List<SelectListItem> SeatingPlanList { get; set; } 

        public string ReturnUrl { get; set; }
    }
} 
