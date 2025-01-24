using GDC.EventHost.Shared.Performance;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class PerformanceListVM
    {
        public string SearchQuery { get; set; } 

        public IEnumerable<PerformanceDetailDto> PerformanceDetails { get; set; } 
    }
}
