using GDC.EventHost.Shared.PerformanceType;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class PerformanceTypeListVM
    {
        public IEnumerable<PerformanceTypeDto> PerformanceTypeList { get; set; }
    }
}