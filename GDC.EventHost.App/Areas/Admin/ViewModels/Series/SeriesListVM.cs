using GDC.EventHost.Shared.Series;
using GDC.EventHost.App.Utilities;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class SeriesListVM
    {
        public string SearchQuery { get; set; }
        public PaginatedList<SeriesDetailDto> SeriesList { get; set; }
    }
}
