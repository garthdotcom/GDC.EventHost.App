using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.ViewModels;
using GDC.EventHost.Shared.Event;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Components
{
    public class EventDetails : ViewComponent
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EventDetails> _logger;
        private readonly IEventHostService _eventHostService;

        public EventDetails(IConfiguration configuration,
            ILogger<EventDetails> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid eventId)
        {
            var eventDetail = await _eventHostService.GetOne<EventDetailDto>($"/events/{eventId}");

            var viewModel = new EventDetailsViewModel
            {
                Event = eventDetail
            };

            return View(viewModel);
        }
    }
}
