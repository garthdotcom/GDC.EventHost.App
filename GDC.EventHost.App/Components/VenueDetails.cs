using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Venue;
using GDC.EventHost.App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Components
{
    public class VenueDetails : ViewComponent
    {
        private readonly IEventHostService _eventHostService;
        private readonly IConfiguration _config;

        public VenueDetails(IConfiguration config, IEventHostService eventHostService)
        {
            _eventHostService = eventHostService;
            _config = config;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid venueId)
        {
            var venueDetail = await _eventHostService.GetOne<VenueDetailDto>($"/venues/{venueId}");

            var viewModel = new VenueDetailsViewModel
            {
                Venue = venueDetail
            };

            return View(viewModel);
        }
    }
}
