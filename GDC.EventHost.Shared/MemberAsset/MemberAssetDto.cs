using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.Shared.MemberAsset
{
    public class MemberAssetDto
    {
        public Guid Id { get; set; }

        public Guid MemberId { get; set; }

        public Guid AssetId { get; set; }

        public AssetTypeEnum AssetTypeId { get; set; }

        public string AssetTypeValue { get; set; } = string.Empty;

        public int OrdinalValue { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Uri { get; set; } = string.Empty;
    }
}
