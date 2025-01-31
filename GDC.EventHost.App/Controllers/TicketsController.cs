using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Services;
using GDC.EventHost.Shared.Ticket;
using GDC.EventHost.App.ViewModels.Tickets;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class TicketsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TicketsController> _logger;
        private readonly IEventHostService _eventHostService;

        public TicketsController(IConfiguration configuration,
            ILogger<TicketsController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> List(Guid performanceId)
        {
            var performanceTicketDtos = await _eventHostService
                .GetMany<TicketDetailDto>($"/performances/{performanceId}/tickets/detail");

            if (performanceTicketDtos == null || performanceTicketDtos.Count() == 0)
            {
                return NotFound();
            }

            var firstTicket = performanceTicketDtos.First();

            var detailedTicketList = new TicketListVM
            {
                PerformanceId = performanceId,
                PerformanceTitle = firstTicket.PerformanceTitle,
                EventDate = firstTicket.PerformanceDate,
                VenueName = firstTicket.VenueName,
                Tickets = performanceTicketDtos
            };

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("TicketListReturnUrl")))
            {
                // save the referring page into session
                HttpContext.Session.SetString("TicketListReturnUrl", 
                    Request.Headers["Referer"].ToString());
            }

            return View(detailedTicketList);
        }

        public ActionResult ReturnToOrigin()
        {
            var originUrl = HttpContext.Session.GetString("TicketListReturnUrl");

            HttpContext.Session.Clear();

            return Redirect(originUrl);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var ticketDetailDto = await _eventHostService
                .GetOne<TicketDetailDto>($"/tickets/{id}");

            // verify object returned contains values
            if (!TryValidateModel(ticketDetailDto, nameof(ticketDetailDto)))
            {
                return RedirectToAction("PageNotFound", "Home");
            }

            var ticketDetailViewModel = new TicketDetailVM
            {
                Ticket = ticketDetailDto
            };

            return View(ticketDetailViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Buy(TicketDetailVM ticketDetailVM)
        {
            var ticketId = ticketDetailVM.Ticket.Id;

            var patchDocList = new List<PatchDocument>()
            {
                new PatchDocument()
                {
                    op = "replace",
                    path = "/ticketStatusId",
                    value = TicketStatusEnum.Sold.ToString()
                }
            };

            string stringData = JsonConvert.SerializeObject(patchDocList);

            // update a single field on the event
            await _eventHostService.PatchOne($"/tickets/{ticketId}", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                ViewBag.Message = "Thank you for your purchase. Enjoy the event!";
            }
            else
            {
                ViewBag.Message = "Sorry, something went wrong. Please try again.";
            }

            return RedirectToAction("List", new { performanceId = ticketDetailVM.Ticket.PerformanceId });
        }
    }
}