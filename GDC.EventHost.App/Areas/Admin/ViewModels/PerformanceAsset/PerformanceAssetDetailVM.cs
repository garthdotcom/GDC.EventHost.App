using GDC.EventHost.Shared.PerformanceAsset;

namespace GDC.EventHost.App.Areas.Admin.ViewModels.PerformanceAsset
{
    public class PerformanceAssetDetailVM
    {
        public PerformanceAssetDto PerformanceAsset { get; set; }

        public string ReturnUrl { get; set; }

        public string PerformanceName { get; set; }
    }
}
