using GDC.EventHost.Shared.SeriesAsset;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeriesAssetEditVM
    {
        public SeriesAssetForUpdateDto SeriesAsset { get; set; }

        public List<SelectListItem> AssetTypeList { get; set; }

        public List<SelectListItem> AssetList { get; set; }

        public string SeriesName { get; set; }
    }
}
