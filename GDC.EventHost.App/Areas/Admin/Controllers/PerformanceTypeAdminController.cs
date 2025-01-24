using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.PerformanceType;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class PerformanceTypeAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PerformanceTypeAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public PerformanceTypeAdminController(IConfiguration configuration,
            ILogger<PerformanceTypeAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> Index()
        {
            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/performancetypes");

            var performanceTypeListViewModel = new PerformanceTypeListVM
            {
                PerformanceTypeList = await _eventHostService
                    .GetMany<PerformanceTypeDto>(uriBuilder.ToString())
            };

            if (performanceTypeListViewModel.PerformanceTypeList.ToList().Count == 0)
            {
                return NotFound();
            }

            return View(performanceTypeListViewModel);
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var performanceType = await _eventHostService
                .GetOne<PerformanceTypeForUpdateDto>($"/performancetypes/{id}");

            var performanceTypeEditViewModel = new PerformanceTypeEditVM
            {
                PerformanceType = performanceType
            };

            return View(performanceTypeEditViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(PerformanceTypeForUpdateDto performanceType)
        {
            string stringData = JsonConvert.SerializeObject(performanceType);

            if (performanceType.Id == Guid.Empty)
            {
                await _eventHostService.PostOne<PerformanceTypeDetailDto>("/performancetypes", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                await _eventHostService.PutOne($"/performancetypes/{performanceType.Id.ToString()}", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }

            var performanceTypeEditViewModel = new PerformanceTypeEditVM
            {
                PerformanceType = performanceType
            };

            return View(performanceTypeEditViewModel);
        }
    }
}