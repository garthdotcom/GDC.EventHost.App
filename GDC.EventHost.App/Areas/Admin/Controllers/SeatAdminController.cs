using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Seat;
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
    public class SeatAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SeatAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public SeatAdminController(IConfiguration configuration,
            ILogger<SeatAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var seatDisplayDto = await _eventHostService.GetOne<SeatDisplayDto>($"/seats/{id}");

            // verify object returned contains values
            if (!TryValidateModel(seatDisplayDto, nameof(seatDisplayDto)))
            {
                return NotFound();  // todo - log this
            }

            var seatDisplayViewModel = new SeatDisplayVM
            {
                SeatDetail = seatDisplayDto,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(seatDisplayViewModel);
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var seatForUpdateDto = await _eventHostService.GetOne<SeatForUpdateDto>($"/seats/{id}");

            var seatEditViewModel = new SeatEditVM
            {
                Seat = seatForUpdateDto,
                SeatTypeList = BuildSeatTypeList(),
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(seatEditViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(SeatEditVM seatEditVM)
        {
            string stringData = JsonConvert.SerializeObject(seatEditVM.Seat);

            if (seatEditVM.Seat.Id == Guid.Empty)
            {
                var newSeat = await _eventHostService.PostOne<SeatDto>("/seats", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = newSeat.Id });
                }
            }
            else
            {
                await _eventHostService.PutOne($"/seats/{seatEditVM.Seat.Id}", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = seatEditVM.Seat.Id });
                }
            }

            var seatEditViewModel = new SeatEditVM
            {
                Seat = seatEditVM.Seat,
                SeatTypeList = BuildSeatTypeList(),
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(seatEditViewModel);
        }


        private List<SelectListItem> BuildSeatTypeList()
        {
            var theList = new List<SelectListItem>();

            var enumValues = Enum.GetValues(typeof(SeatTypeEnum));
            var enumNames = Enum.GetNames(typeof(SeatTypeEnum));

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