using GDC.EventHost.Shared.Performance;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.ViewModels.Performances
{
    public class PerformanceEditVM
    {
        public PerformanceForUpdateDto PerformanceForUpdate { get; set; } 

        public List<SelectListItem> VenueList { get; set; }

        public List<SelectListItem> EventList { get; set; }

        public List<SelectListItem> PerformanceTypesList { get; set; }

        public string PerformanceTitle { get; set; } 
    }
}
