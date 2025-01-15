namespace GDC.EventHost.Shared.Asset
{
    public class AssetDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string Uri { get; set; }
    }
}
