using GDC.EventHost.Shared.Event;
using GDC.EventHost.Shared.Series;

namespace GDC.EventHost.App.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<SeriesDetailDto> SeriesList { get; set; }
        public List<EventDetailDto> FeaturedEvents { get; set; }
    }
} 