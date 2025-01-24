using GDC.EventHost.Shared.Asset;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class AssetDetailVM
    {
        public AssetDetailDto Asset { get; set; } 

        public string ReturnUrl { get; set; }
    }
}
