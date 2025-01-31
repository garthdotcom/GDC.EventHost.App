using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Ticket;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize(Policy = "IsAdministrator")]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class TicketAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TicketAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public TicketAdminController(IConfiguration configuration,
            ILogger<TicketAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var ticketDetailDto = await _eventHostService.GetOne<TicketDetailDto>($"/tickets/{id}");

            // verify object returned contains values
            if (!TryValidateModel(ticketDetailDto, nameof(ticketDetailDto)))
            {
                return NotFound();  // todo - create a friendly not found page
            }

            var ticketDetailViewModel = new TicketDetailVM
            {
                Ticket = ticketDetailDto,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(ticketDetailViewModel);
        }


        public async Task<ActionResult> Edit(Guid id)
        {
            var ticketForUpdateDto = await _eventHostService
                .GetOne<TicketForUpdateDto>($"/tickets/{id}");

            var ticketEditViewModel = new TicketEditVM
            {
                Ticket = ticketForUpdateDto,
                TicketStatusList = BuildTicketStatusList(),
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(ticketEditViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(TicketEditVM ticketEditVM)
        {
            string stringData = JsonConvert.SerializeObject(ticketEditVM.Ticket);

            if (ticketEditVM.Ticket.Id == Guid.Empty)
            {
                var newTicket = await _eventHostService
                    .PostOne<TicketDetailDto>("/tickets", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = newTicket.Id });
                }
            }
            else
            {
                await _eventHostService.PutOne($"/tickets/{ticketEditVM.Ticket.Id}", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = ticketEditVM.Ticket.Id });
                }
            }

            var ticketEditViewModel = new TicketEditVM
            {
                Ticket = ticketEditVM.Ticket,
                TicketStatusList = BuildTicketStatusList(),
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(ticketEditViewModel);
        }


        private List<SelectListItem> BuildTicketStatusList()
        {
            var theList = new List<SelectListItem>();

            var enumValues = Enum.GetValues(typeof(TicketStatusEnum));
            var enumNames = Enum.GetNames(typeof(TicketStatusEnum));

            var nameCount = 0;
            foreach (var value in enumValues)
            {
                theList.Add(new SelectListItem { Value = value.ToString(), Text = enumNames[nameCount] });
                nameCount++;
            }

            return theList;
        }
    }
}