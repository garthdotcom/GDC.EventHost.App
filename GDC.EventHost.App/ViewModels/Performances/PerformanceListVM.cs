using GDC.EventHost.Shared.Performance;

namespace GDC.EventHost.App.ViewModels.Performances
{
    public class PerformanceListVM
    {
        public string SearchQuery { get; set; } 

        public IEnumerable<PerformanceDetailDto> PerformanceDetails { get; set; } 
    }
}
