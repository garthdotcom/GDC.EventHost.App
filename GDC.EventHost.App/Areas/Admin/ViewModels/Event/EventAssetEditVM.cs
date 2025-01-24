using GDC.EventHost.Shared.EventAsset;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class EventAssetEditVM
    {
        public EventAssetForUpdateDto EventAsset { get; set; }

        public List<SelectListItem> AssetTypeList { get; set; }

        public List<SelectListItem> AssetList { get; set; }

        public string EventTitle { get; set; }  
    } 
}
