using GDC.EventHost.Shared.PerformanceAsset;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels.PerformanceAsset
{
    public class PerformanceAssetEditVM
    {
        public PerformanceAssetForUpdateDto PerformanceAsset { get; set; }

        public List<SelectListItem> AssetTypeList { get; set; }

        public List<SelectListItem> AssetList { get; set; }

        public string PerformanceName { get; set; }
    }
}
