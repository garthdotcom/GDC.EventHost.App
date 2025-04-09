using GDC.EventHost.Shared.Event;
using GDC.EventHost.Shared.EventAsset;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDC.EventHost.App.Areas.Admin.ViewModels
{
    public class EventEditVM 
    {
        public Guid Id { get; set; }

        public EventForUpdateDto Event { get; set; }

        public List<EventAssetDto> EventAssets { get; set; } = [];

        public List<SelectListItem> SeriesList { get; set; } = [];

        public IFormFile? FileUpload { get; set; }
    }
}
