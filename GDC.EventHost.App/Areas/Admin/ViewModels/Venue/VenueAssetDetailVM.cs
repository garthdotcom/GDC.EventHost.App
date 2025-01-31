using GDC.EventHost.Shared.VenueAsset;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class VenueAssetDetailVM
    {
        public VenueAssetDto VenueAsset { get; set; }

        public string ReturnUrl { get; set; }

        public string VenueName { get; set; }
    }
}
