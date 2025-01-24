using GDC.EventHost.Shared.EventAsset;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class EventAssetDetailVM
    {
        public EventAssetDto EventAsset { get; set; }

        public string ReturnUrl { get; set; }

        public string EventTitle { get; set; }
    }
}
