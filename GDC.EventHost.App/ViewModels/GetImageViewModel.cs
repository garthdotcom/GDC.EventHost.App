using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.ViewModels
{
    public class GetImageViewModel
    {
        public string Name { get; set; }

        public string Uri { get; set; }

        public AssetTypeEnum AssetType { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}
