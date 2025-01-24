using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Event;
using GDC.EventHost.Shared.Series;
using GDC.EventHost.Shared.Venue;
using GDC.EventHost.App.Utilities;
using GDC.EventHost.App.ViewModels.Events;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class EventsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EventsController> _logger;
        private readonly IEventHostService _eventHostService;

        public EventsController(IConfiguration configuration,
            ILogger<EventsController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> List(string searchQuery, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchQuery; 

            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/events");

            if (!string.IsNullOrEmpty(searchQuery) && searchQuery.ToLowerInvariant() != "all")
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var summaryDtos = await _eventHostService
                .GetMany<EventDetailDto>(uriBuilder.ToString());

            int pageSize = 3;

            // todo: move the orderby to the api

            var eventList = new PaginatedList<EventDetailDto>(summaryDtos
                .OrderBy(s => s.EndDate)
                .AsQueryable(),
                pageNumber ?? 1, pageSize);

            //********************************************************************************

            var seriesNames = new List<string>();

            var allSeries = await _eventHostService.GetMany<SeriesDto>($"/series");

            if (allSeries.Count() > 0)
            {
                var currentSeries = allSeries
                    .Where(s => !s.EndDate.HasValue || s.EndDate.Value >= DateTime.Now)
                    .OrderBy(s => s.Title);

                foreach (var series in currentSeries)
                {
                    seriesNames.Add(series.Title);
                }
            }

            var venueNames = new List<string>();

            var allVenues = await _eventHostService.GetMany<VenueDto>($"/venues");

            if (allVenues.Count() > 0)
            {
                var currentVenue = allVenues
                    .OrderBy(v => v.Name);

                foreach (var venue in currentVenue)
                {
                    venueNames.Add(venue.Name);
                }
            }

            var monthNames = new List<string>();

            var currentMonth = DateTime.Now.Month;

            for (int i = 1; i <= 12; i++)
            {
                monthNames.Add(CultureInfo.CurrentCulture.DateTimeFormat
                    .GetAbbreviatedMonthName(currentMonth));
                currentMonth = (currentMonth == 12) ? 1 : currentMonth + 1;
            }

            //********************************************************************************

            var eventListViewModel = new EventListVM
            {
                EventList = eventList,
                SeriesNames = seriesNames,
                VenueNames = venueNames,
                MonthNames = monthNames
            };

            return View(eventListViewModel);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var eventDetail = await _eventHostService.GetOne<EventDetailDto>($"/events/{id}");

            var eventDetailViewModel = new EventDetailVM
            {
                Event = eventDetail,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(eventDetailViewModel);
        }

    }
}