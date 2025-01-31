using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Services;
using GDC.EventHost.Shared.SeatingPlan;
using GDC.EventHost.Shared.SeatPosition;
using GDC.EventHost.Shared.Venue;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using GDC.EventHost.App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    // during the seating plan creation process we want to build up a list of SeatPositionsForCreateDto objects
    // which can be included in the SeatingPlanForCreateDto when we are ready to create.

    [Authorize(Policy = "IsAdministrator")]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class SeatingPlanAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SeatingPlanAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public SeatingPlanAdminController(IConfiguration configuration,
            ILogger<SeatingPlanAdminController> logger,
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
            ViewData["SeatingPlanNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "seatingPlan_name_desc" : "";
            ViewData["VenueNameSortParm"] = sortOrder == "VenueName" ? "venue_name_desc" : "VenueName";

            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/seatingplans");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            // user may choose to create a new layout. clear the session so no leftover positions exist
            HttpContext.Session.Clear();

            var seatingPlanDtos = await _eventHostService.GetMany<SeatingPlanDetailDto>(uriBuilder.ToString());

            switch (sortOrder)
            {
                case "seatingPlan_name_desc":
                    seatingPlanDtos = seatingPlanDtos.OrderByDescending(s => s.Name);
                    break;
                case "VenueName":
                    seatingPlanDtos = seatingPlanDtos.OrderBy(s => s.VenueName);
                    break;
                case "venue_name_desc":
                    seatingPlanDtos = seatingPlanDtos.OrderByDescending(s => s.VenueName);
                    break;
                default:
                    seatingPlanDtos = seatingPlanDtos.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;

            var seatingPlanList = new PaginatedList<SeatingPlanDetailDto>(seatingPlanDtos.AsQueryable(),
                pageNumber ?? 1, pageSize);

            var seatingPlanListViewModel = new SeatingPlanListVM
            {
                SeatingPlanList = seatingPlanList
            };

            return View(seatingPlanListViewModel);
        }


        public async Task<ActionResult> Detail(Guid id, Guid? performanceId = null)
        {
            var seatingPlanDetailDto = await _eventHostService
                .GetOne<SeatingPlanDetailDto>($"/seatingplans/{id}");

            // verify object returned contains values
            if (!TryValidateModel(seatingPlanDetailDto, nameof(seatingPlanDetailDto)))
            {
                return NotFound();  // todo - create a friendly not found page
            }

            var seatingPlanDetailViewModel = new SeatingPlanDetailVM
            {
                SeatingPlanDetail = seatingPlanDetailDto,
                ReturnUrl = Request.Headers["Referer"].ToString(),
                PerformanceId = performanceId
            };

            return View(seatingPlanDetailViewModel);
        }

        public async Task<ActionResult> Add(Guid? performanceId = null)
        {
            var savedPositions = HttpContext.Session.GetString("Positions");

            var viewModel = new SeatingPlanAddVM
            {
                PositionsCreated = !string.IsNullOrEmpty(savedPositions),
                PerformanceId = performanceId
            };

            if (!string.IsNullOrEmpty(savedPositions))
            {
                viewModel.VenueList = await BuildVenueList();
                //viewModel.SeatingPlan.SeatPositions = GetSeatPositionsFromSessionObject();
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Add(SeatingPlanAddVM seatingPlanAddVM) 
        {
            seatingPlanAddVM.SeatingPlan.SeatPositions = GetSeatPositionsFromSessionObject();

            string stringData = JsonConvert.SerializeObject(seatingPlanAddVM.SeatingPlan);

            var seatingPlanDetailDto = await _eventHostService
                .PostOne<SeatingPlanDetailDto>("/seatingplans", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                HttpContext.Session.Clear(); // remove the positions list from memory

                if (seatingPlanAddVM.PerformanceId.HasValue)
                {
                    // try to add the new seating plan to the performance

                    var patchDocList = new List<PatchDocument>()
                    {
                        new PatchDocument()
                        {
                            op = "replace",
                            path = "/seatingPlanId",
                            value = seatingPlanDetailDto.Id.ToString()
                        }
                    };

                    stringData = JsonConvert.SerializeObject(patchDocList);

                    await _eventHostService
                        .PatchOne($"/performances/{seatingPlanAddVM.PerformanceId.Value}", stringData);

                    if (_eventHostService.Error)
                    {
                        ModelState.AddModelError("", _eventHostService.Messages);
                    }
                    else
                    {
                        return RedirectToAction("Detail", new { id = seatingPlanDetailDto.Id, performanceId = seatingPlanAddVM.PerformanceId });
                    }
                }
                else
                {
                    return RedirectToAction("Detail", new { id = seatingPlanDetailDto.Id, performanceId = seatingPlanAddVM.PerformanceId });
                }
            }

            // add was not successful, remain on the page

            var savedPositions = HttpContext.Session.GetString("Positions");

            var viewModel = new SeatingPlanAddVM
            {
                PositionsCreated = !string.IsNullOrEmpty(savedPositions)
            };

            if (!string.IsNullOrEmpty(savedPositions))
            {
                viewModel.VenueList = await BuildVenueList();
                viewModel.SeatingPlan.SeatPositions = GetSeatPositionsFromSessionObject();
            }

            return View(viewModel);
        }

        private List<SeatPositionsForCreateDto> GetSeatPositionsFromSessionObject()
        {
            var seatPositions = new List<SeatPositionsForCreateDto>();
            var nextLevel = 0;

            var savedPositions = HttpContext.Session.GetString("Positions");

            if (!string.IsNullOrEmpty(savedPositions))
            {
                var positionArray = savedPositions.Split("|");
                foreach (var posn in positionArray)
                {
                    var work = posn.Split(",");
                    if (work.Length == 3)
                    {
                        seatPositions.Add(new SeatPositionsForCreateDto()
                        {
                            Level = Convert.ToInt32(work[0]),
                            Number = Convert.ToInt32(work[1]),
                            SeatPositionType = (SeatPositionTypeEnum)Enum.Parse(typeof(SeatPositionTypeEnum), work[2])
                        });
                        nextLevel++;
                    }
                }
            }

            return seatPositions;
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

        // we can arrive here from Performance Detail or SeatingPlan Index
        public IActionResult AddPosition(Guid? performanceId = null)
        {
            var seatPositions = GetSeatPositionsFromSessionObject();

            var viewModel = new SeatPositionEditVM
            {
                Positions = seatPositions,
                Level = seatPositions.Count,
                Number = 0,
                PerformanceId = performanceId
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddPosition(SeatPositionEditVM seatPositionEditVM)
        {
            var savedPositions = HttpContext.Session.GetString("Positions");

            var newPosition = $"{seatPositionEditVM.Level},{seatPositionEditVM.Number},{seatPositionEditVM.SeatPositionTypeId}";

            HttpContext.Session.SetString("Positions", savedPositions + "|" + newPosition);

            return RedirectToAction("AddPosition", new { performanceId = seatPositionEditVM.PerformanceId });
        }

        public IActionResult ResetPositions(Guid? performanceId)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("AddPosition", new { performanceId = performanceId.Value });
        }
    }
}