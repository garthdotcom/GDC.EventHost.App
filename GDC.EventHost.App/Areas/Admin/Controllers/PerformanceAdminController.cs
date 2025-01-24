using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Services;
using GDC.EventHost.Shared.Event;
using GDC.EventHost.Shared.Performance;
using GDC.EventHost.Shared.PerformanceType;
using GDC.EventHost.Shared.SeatingPlan;
using GDC.EventHost.Shared.Ticket;
using GDC.EventHost.Shared.Venue;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using GDC.EventHost.App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class PerformanceAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PerformanceAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public PerformanceAdminController(IConfiguration configuration,
            ILogger<PerformanceAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> List(string searchQuery)
        {
            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/performances");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var eventListViewModel = new PerformanceListVM
            {
                PerformanceDetails = await _eventHostService.GetMany<PerformanceDetailDto>(uriBuilder.ToString())
            };

            return View(eventListViewModel);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var performanceDetail = await _eventHostService.GetOne<PerformanceDetailDto>($"/performances/{id}");

            // verify object returned contains values
            if (!TryValidateModel(performanceDetail, nameof(performanceDetail)))
            {
                return NotFound();  // todo - log this
            }

            // user may choose to create a new seatingPlan. clear the session so no leftover positions exist
            HttpContext.Session.Clear();

            var performanceDetailViewModel = new PerformanceDetailVM
            {
                PerformanceDetail = performanceDetail,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(performanceDetailViewModel);
        }

        public async Task<ActionResult> Edit(Guid id, Guid eventId)
        {
            var performanceForUpdate = await _eventHostService.GetOne<PerformanceForUpdateDto>($"/performances/{id}");

            // unable to add a seatingPlan here since the venue can be changed and we are not using extra js
            // add an extra route from the detail page where this is static

            var performanceEditViewModel = new PerformanceEditVM
            {
                PerformanceForUpdate = performanceForUpdate,
                EventList = await BuildEventList(eventId),
                PerformanceTypesList = await BuildPerformanceTypesList(),
                VenueList = await BuildVenueList(),
                PerformanceTitle = await GetEventTitle(eventId)
            };

            if (id == Guid.Empty)
            {
                performanceEditViewModel.PerformanceForUpdate.Date = DateTime.Now.WithoutSeconds();
                performanceEditViewModel.PerformanceForUpdate.EventId = eventId;
            }

            return View(performanceEditViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(PerformanceForUpdateDto performanceForUpdate)
        {
            string stringData = JsonConvert.SerializeObject(performanceForUpdate);

            if (performanceForUpdate.Id == Guid.Empty)
            {
                var newEvent = await _eventHostService.PostOne<PerformanceDetailDto>("/performances", stringData);

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
                await _eventHostService.PutOne($"/performances/{performanceForUpdate.Id}", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = performanceForUpdate.Id });
                }
            }

            var performanceEditViewModel = new PerformanceEditVM
            {
                PerformanceForUpdate = performanceForUpdate,
                EventList = await BuildEventList(),
                PerformanceTypesList = await BuildPerformanceTypesList(),
                VenueList = await BuildVenueList(),
                PerformanceTitle = await GetEventTitle(performanceForUpdate.EventId)
            };

            return View(performanceEditViewModel);
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            var performanceToDelete = await _eventHostService.GetOne<PerformanceDetailDto>($"/performances/{id}");

            if (performanceToDelete == null)
            {
                return NotFound();
            }

            await _eventHostService.DeleteOne($"/performances/{id}");

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            return RedirectToAction("Detail", "EventAdmin", new { id = performanceToDelete.EventId });
        }

        public async Task<ActionResult> CreateTickets(Guid id)
        {
            var performanceDto = await _eventHostService.GetOne<PerformanceDetailDto>($"/performances/{id}");

            if (performanceDto == null)
            {
                return NotFound();
            }

            if (performanceDto.SeatingPlanId == null || performanceDto.SeatingPlanId == Guid.Empty)
            {
                ModelState.AddModelError("A SeatingPlan must be chosen before tickets can be created.", _eventHostService.Messages);
            }

            var performanceTicketTemplateVM = new PerformanceTicketTemplateForCreateVM
            {
                PerformanceTicketForCreate = new PerformanceTicketForCreateDto()
                {
                    PerformanceId = id,
                    SeatingPlanId = performanceDto.SeatingPlanId.Value
                },
                PerformanceTitle = performanceDto.EventTitle,
                SeatingPlanName = performanceDto.SeatingPlanName,
                ReturnUrl = Request.Headers["Referer"].ToString()
            }; 
             
            return View(performanceTicketTemplateVM);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTickets(PerformanceTicketTemplateForCreateVM performanceTicketTemplateForCreateVM)
        {
            string stringData = JsonConvert.SerializeObject(performanceTicketTemplateForCreateVM.PerformanceTicketForCreate);

            var performanceId = performanceTicketTemplateForCreateVM.PerformanceTicketForCreate.PerformanceId;

            await _eventHostService.PostOne<IEnumerable<TicketDto>>($"/performances/{performanceId}/tickets", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("DisplayTickets", new { id = performanceId });
            }

            return View(performanceTicketTemplateForCreateVM);
        }

        public async Task<ActionResult> DisplayTickets(Guid id)
        {
            var performanceTicketDtos = await _eventHostService
                .GetMany<TicketDetailDto>($"/performances/{id}/tickets/detail");

            if (performanceTicketDtos == null || performanceTicketDtos.Count() == 0)
            {
                return NotFound();
            }

            var firstTicket = performanceTicketDtos.First();

            var detailedTicketList = new PerformanceTicketListVM
            {
                PerformanceId = id,
                PerformanceTitle = firstTicket.PerformanceTitle,
                PerformanceDate = firstTicket.PerformanceDate,
                VenueName = firstTicket.VenueName,
                Tickets = performanceTicketDtos,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(detailedTicketList);
        }
      
        public async Task<ActionResult> AddSeatingPlanToPerformance(Guid id)
        {
            var performanceDto = await _eventHostService.GetOne<PerformanceDetailDto>($"/performances/{id}");

            if (performanceDto == null)
            {
                return NotFound();
            }

            var seatingPlanList = performanceDto.VenueId.HasValue ? 
                await BuildSeatingPlanList(performanceDto.VenueId.Value) : 
                new List<SelectListItem>();

            var addSeatingPlanVM = new AddSeatingPlanToPerformanceVM
            {
                PerformanceDetail = performanceDto,
                SeatingPlanList = seatingPlanList,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(addSeatingPlanVM);
        }

        [HttpPost]
        public async Task<ActionResult> AddSeatingPlanToPerformance(AddSeatingPlanToPerformanceVM addSeatingPlanToEventVM)
        {
            var performanceId = addSeatingPlanToEventVM.PerformanceDetail.Id;
            var seatingPlanId = addSeatingPlanToEventVM.PerformanceDetail.SeatingPlanId;

            var patchDocList = new List<PatchDocument>()
            {
                new PatchDocument()
                {
                    op = "replace",
                    path = "/seatingPlanId",
                    value = seatingPlanId.Value.ToString()
                }
            };

            string stringData = JsonConvert.SerializeObject(patchDocList);

            // update a single field on the event
            await _eventHostService.PatchOne($"/performances/{performanceId}", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Detail", new { id = performanceId });
            }

            return View(addSeatingPlanToEventVM);
        }


        private async Task<List<SelectListItem>> BuildEventList(Guid? eventId = null)
        {
            var events = await _eventHostService.GetMany<EventDto>("/events");

            var theList = new List<SelectListItem>();

            foreach (var evt in events)
            {
                var listItem = new SelectListItem { Value = evt.Id.ToString(), Text = evt.Title };

                if (eventId.HasValue && eventId == evt.Id)
                {
                    listItem.Selected = true;
                }

                theList.Add(listItem);
            }

            return theList;
        }

        private async Task<List<SelectListItem>> BuildVenueList()
        {
            var venues = await _eventHostService.GetMany<VenueDto>("/venues");

            var theList = new List<SelectListItem>();

            foreach (var venue in venues)
            {
                theList.Add(new SelectListItem { Value = venue.Id.ToString(), Text = venue.Name });
            }

            return theList;
        }

        private async Task<List<SelectListItem>> BuildSeatingPlanList(Guid venueId)
        {
            var seatingPlansForVenue = await _eventHostService.GetMany<SeatingPlanDto>($"/venues/{venueId}/seatingPlans");

            var theList = new List<SelectListItem>();

            foreach (var seatingPlan in seatingPlansForVenue)
            {
                theList.Add(new SelectListItem { Value = seatingPlan.Id.ToString(), Text = seatingPlan.Name });
            }

            return theList;
        }

        private async Task<List<SelectListItem>> BuildPerformanceTypesList()
        {
            var performanceTypes = await _eventHostService.GetMany<PerformanceTypeDto>("/performancetypes");

            var theList = new List<SelectListItem>();

            foreach (var performanceTypeItem in performanceTypes)
            {
                theList.Add(new SelectListItem { Value = performanceTypeItem.Id.ToString(), Text = performanceTypeItem.Name });
            }

            return theList;
        }

        private async Task<string> GetEventTitle(Guid eventId)
        {
            var eventDto = await _eventHostService
                .GetOne<EventDto>($"/events/{eventId}");

            return eventDto.Title;
        }
    }
}