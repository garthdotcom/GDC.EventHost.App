using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Venue;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using GDC.EventHost.App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class VenueAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VenueAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public VenueAdminController(IConfiguration configuration,
            ILogger<VenueAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> Index(string searchQuery, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchQuery;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DescriptionSortParm"] = sortOrder == "Description" ? "description_desc" : "Description";

            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/venues");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var venueDtos = await _eventHostService.GetMany<VenueDetailDto>(uriBuilder.ToString());

            switch (sortOrder)
            {
                case "name_desc":
                    venueDtos = venueDtos.OrderByDescending(s => s.Name);
                    break;
                case "Description":
                    venueDtos = venueDtos.OrderBy(s => s.Description);
                    break;
                case "description_desc":
                    venueDtos = venueDtos.OrderByDescending(s => s.Description);
                    break;
                default:
                    venueDtos = venueDtos.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;

            var venueList = new PaginatedList<VenueDetailDto>(venueDtos.AsQueryable(),
                pageNumber ?? 1, pageSize);

            var venueListViewModel = new VenueListVM
            {
                VenueList = venueList
            };

            return View(venueListViewModel);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var venueDetail = await _eventHostService.GetOne<VenueDetailDto>($"/venues/{id}");

            // verify object returned contains values
            if (!TryValidateModel(venueDetail, nameof(venueDetail)))
            {
                return NotFound();  // todo - create a friendly not found page
            }

            var venueDetailViewModel = new VenueDetailVM
            {
                Venue = venueDetail,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(venueDetailViewModel);
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var venue = await _eventHostService.GetOne<VenueForUpdateDto>($"/venues/{id}");

            var venueEditViewModel = new VenueEditVM
            {
                Venue = venue
            };

            return View(venueEditViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(VenueForUpdateDto venue)
        {
            string stringData = JsonConvert.SerializeObject(venue);

            if (venue.Id == Guid.Empty)
            {
                var newVenue = await _eventHostService.PostOne<VenueDetailDto>("/venues", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = newVenue.Id });
                }
            }
            else
            {
                await _eventHostService.PutOne($"/venues/{venue.Id.ToString()}", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = venue.Id });
                }
            }

            var venueEditViewModel = new VenueEditVM
            {
                Venue = venue
            };

            return View(venueEditViewModel);
        }

    }
}