using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Areas.Admin.ViewModels.PerformanceAsset;
using GDC.EventHost.Shared.Asset;
using GDC.EventHost.Shared.Performance;
using GDC.EventHost.Shared.PerformanceAsset;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize(Policy = "IsAdministrator")]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class PerformanceAssetAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PerformanceAssetAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public PerformanceAssetAdminController(IConfiguration configuration,
            ILogger<PerformanceAssetAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        // displays the specific asset
        public async Task<ActionResult> Detail(Guid id)
        {
            var performanceAssetDto = await _eventHostService
                .GetOne<PerformanceAssetDto>($"/performanceassets/{id}");

            // verify object returned contains values
            if (!TryValidateModel(performanceAssetDto, nameof(performanceAssetDto)))
            {
                return RedirectToAction("NotFound", "Home");
            }

            var performanceAssetDetailVM = new PerformanceAssetDetailVM
            {
                PerformanceAsset = performanceAssetDto,
                ReturnUrl = Request.Headers["Referer"].ToString(),
                PerformanceName = await GetPerformanceName(performanceAssetDto.PerformanceId)
            };

            return View(performanceAssetDetailVM);
        }

        public async Task<ActionResult> Add(Guid performanceId)
        {
            var performanceAssetForUpdateDto = new PerformanceAssetForUpdateDto()
            {
                PerformanceId = performanceId
            };

            var assetEditViewModel = new PerformanceAssetEditVM
            {
                PerformanceAsset = performanceAssetForUpdateDto,
                AssetList = await BuildAssetList(),
                PerformanceName = await GetPerformanceName(performanceId)
            };

            return View(assetEditViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Add(PerformanceAssetEditVM performanceAssetEditVM)
        {
            string stringData = JsonConvert.SerializeObject(performanceAssetEditVM.PerformanceAsset);

            var performanceAssetDto = await _eventHostService
                .PostOne<PerformanceAssetDto>($"/performances/{performanceAssetEditVM.PerformanceAsset.PerformanceId}/assets", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Detail", "PerformanceAdmin", new { id = performanceAssetDto.PerformanceId });
            }

            var assetEditViewModel = new PerformanceAssetEditVM
            {
                PerformanceAsset = performanceAssetEditVM.PerformanceAsset,
                AssetList = await BuildAssetList(),
                PerformanceName = await GetPerformanceName(performanceAssetEditVM.PerformanceAsset.PerformanceId)
            };

            return View(assetEditViewModel);
        }


        public async Task<ActionResult> Delete(Guid id)
        {
            var assetToDelete = await _eventHostService.GetOne<PerformanceAssetDto>($"/performanceassets/{id}");

            if (assetToDelete == null)
            {
                return NotFound();
            }

            await _eventHostService.DeleteOne($"/performanceassets/{id}");

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            return RedirectToAction("Detail", "PerformanceAdmin", new { id = assetToDelete.PerformanceId });
        }

        private async Task<List<SelectListItem>> BuildAssetList()
        {
            var allAssets = await _eventHostService.GetMany<AssetDto>("/assets");

            var theList = new List<SelectListItem>();

            foreach (var asset in allAssets)
            {
                theList.Add(new SelectListItem { Value = asset.Id.ToString(), Text = asset.Name });
            }

            return theList;
        }

        private async Task<string> GetPerformanceName(Guid performanceId)
        {
            var performanceDto = await _eventHostService
                .GetOne<PerformanceDetailDto>($"/performances/{performanceId}");

            return performanceDto.EventTitle;
        }
    }
}