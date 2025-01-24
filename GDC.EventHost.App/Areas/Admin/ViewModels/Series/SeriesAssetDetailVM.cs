using GDC.EventHost.Shared.SeriesAsset;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeriesAssetDetailVM
    {
        public SeriesAssetDto SeriesAsset { get; set; }

        public string ReturnUrl { get; set; }

        public string SeriesName { get; set; } 
    }
}
