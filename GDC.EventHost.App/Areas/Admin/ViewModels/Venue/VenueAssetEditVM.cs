using GDC.EventHost.Shared.VenueAsset;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class VenueAssetEditVM
    {
        public VenueAssetForUpdateDto VenueAsset { get; set; }

        public List<SelectListItem> AssetTypeList { get; set; }

        public List<SelectListItem> AssetList { get; set; }

        public string VenueName { get; set; }
    }
}
