using GDC.EventHost.Shared.Asset;
using GDC.EventHost.App.Utilities;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class AssetListVM
    {
        public string SearchQuery { get; set; }
        public PaginatedList<AssetDetailDto> AssetList { get; set; }
    }
}
