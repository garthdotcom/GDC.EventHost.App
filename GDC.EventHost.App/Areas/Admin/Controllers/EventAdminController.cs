using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Event;
using GDC.EventHost.Shared.Series;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using GDC.EventHost.App.Components;
using GDC.EventHost.App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize(Policy = "IsAdministrator")]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class EventAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EventAdminController> _logger;
        private readonly IEventHostService _eventHostService;
        private readonly IEventAssetStorage _artStorage;

        public EventAdminController(IConfiguration configuration,
            ILogger<EventAdminController> logger,
            IEventHostService eventHostService,
            IEventAssetStorage artStorage)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
            _artStorage = artStorage;
        }

        public async Task<ActionResult> Index(string searchQuery, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchQuery;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EventNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "event_name_desc" : "";
            ViewData["SeriesNameSortParm"] = sortOrder == "SeriesName" ? "series_name_desc" : "SeriesName";

            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/events");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery="); 
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var eventDtos = await _eventHostService.GetMany<EventDetailDto>(uriBuilder.ToString());

            switch (sortOrder)
            {
                case "event_name_desc":
                    eventDtos = eventDtos.OrderByDescending(s => s.Title);
                    break;
                case "SeriesName":
                    eventDtos = eventDtos.OrderBy(s => s.SeriesTitle);
                    break;
                case "series_name_desc":
                    eventDtos = eventDtos.OrderByDescending(s => s.SeriesTitle);
                    break;
                default:
                    eventDtos = eventDtos.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 10;

            var eventList = new PaginatedList<EventDetailDto>(eventDtos.AsQueryable(), 
                pageNumber ?? 1, pageSize);

            var eventListViewModel = new EventListVM
            {
                EventList = eventList
            };

            return View(eventListViewModel);
        }


        public async Task<ActionResult> Detail(Guid id)
        {
            var eventDetail = await _eventHostService.GetOne<EventDetailDto>($"/events/{id}");

            // verify object returned contains values
            if (!TryValidateModel(eventDetail, nameof(eventDetail)))
            {
                return NotFound();  // todo - create a friendly not found page
            }

            var eventDetailViewModel = new EventDetailVM
            {
                Event = eventDetail,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(eventDetailViewModel);
        }


        public async Task<ActionResult> Edit(Guid id)
        {
            var evt = await _eventHostService.GetOne<EventForUpdateDto>($"/events/{id}");

            var eventEditViewModel = new EventEditVM
            {
                Event = evt,
                SeriesList = await BuildSeriesList()
            };

            return View(eventEditViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(EventEditVM eventVM)
        {
            string stringData = JsonConvert.SerializeObject(eventVM.Event);

            if (eventVM.Event.Id == Guid.Empty)
            {
                var newEvent = await _eventHostService.PostOne<EventForUpdateDto>("/events", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = newEvent.Id });
                }
            }
            else
            {
                await _eventHostService.PutOne($"/events/{eventVM.Event.Id}", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = eventVM.Event.Id });
                }
            }

            var eventEditViewModel = new EventEditVM
            {
                Event = eventVM.Event,
                SeriesList = await BuildSeriesList()
            };

            return View(eventEditViewModel);
        }


        private async Task<List<SelectListItem>> BuildSeriesList()
        {
            var series = await _eventHostService.GetMany<SeriesDto>("/series");

            var theList = new List<SelectListItem>();

            foreach (var seriesItem in series)
            {
                theList.Add(new SelectListItem { Value = seriesItem.Id.ToString(), Text = seriesItem.Title });
            }

            return theList;
        }
    }
}