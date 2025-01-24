using GDC.EventHost.Shared.Event;
using GDC.EventHost.Shared.EventAsset;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.ViewModels.Events
{
    public class EventEditVM 
    {
        public EventForUpdateDto Event { get; set; }

        public List<EventAssetDto> EventAssets { get; set; }

        public List<SelectListItem> SeriesList { get; set; }

        public IFormFile FileUpload { get; set; }
    }
}
