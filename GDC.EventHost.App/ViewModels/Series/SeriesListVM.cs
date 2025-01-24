using GDC.EventHost.Shared.Series;

namespace GDC.EventHost.App.ViewModels.Series
{
    public class SeriesListVM
    {
        public string SearchQuery { get; set; }
        public IEnumerable<SeriesDetailDto> SeriesList { get; set; }
    }
}
