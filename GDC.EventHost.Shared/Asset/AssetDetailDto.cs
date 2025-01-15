using GDC.EventHost.Shared.EventAsset;
using GDC.EventHost.Shared.PerformanceAsset;
using GDC.EventHost.Shared.SeriesAsset;
using GDC.EventHost.Shared.VenueAsset;

namespace GDC.EventHost.Shared.Asset
{
    public class AssetDetailDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string Uri { get; set; }

        public List<SeriesAssetDto> SeriesAssets { get; set; } = [];

        public List<EventAssetDto> EventAssets { get; set; } = [];

        public List<PerformanceAssetDto> PerformanceAssets { get; set; } = [];

        public List<VenueAssetDto> VenueAssets { get; set; } = [];
    }
}
