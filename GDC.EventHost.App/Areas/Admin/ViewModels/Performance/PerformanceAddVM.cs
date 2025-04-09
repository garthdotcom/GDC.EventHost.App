using GDC.EventHost.Shared.Performance;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels.Performance
{
    public class PerformanceAddVM
    {
        public PerformanceForCreateDto PerformanceForCreate { get; set; }

        public List<SelectListItem> VenueList { get; set; }

        public List<SelectListItem> PerformanceTypesList { get; set; }

        public string PerformanceTitle { get; set; }
    }
}
