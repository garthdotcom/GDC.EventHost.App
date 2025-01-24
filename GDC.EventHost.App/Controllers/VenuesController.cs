using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Venue;
using GDC.EventHost.App.ViewModels.Venues;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class VenuesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VenuesController> _logger;
        private readonly IEventHostService _eventHostService;

        public VenuesController(IConfiguration configuration,
            ILogger<VenuesController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> List(string searchQuery)
        {
            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/venues");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var venueListViewModel = new VenueListVM
            {
                VenueList = await _eventHostService
                    .GetMany<VenueDetailDto>(uriBuilder.ToString())
            };

            return View(venueListViewModel);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var venueDetail = await _eventHostService
                .GetOne<VenueDetailDto>($"/venues/{id}");

            //// verify object returned contains values
            //if (!TryValidateModel(venueDetail, nameof(venueDetail)))
            //{
            //    return NotFound();  // todo - log this
            //}

            var venueDetailViewModel = new VenueDetailVM
            {
                Venue = venueDetail,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(venueDetailViewModel);
        }
    }
}