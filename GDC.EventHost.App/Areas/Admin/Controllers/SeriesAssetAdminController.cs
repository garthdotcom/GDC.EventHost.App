using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Asset;
using GDC.EventHost.Shared.Series;
using GDC.EventHost.Shared.SeriesAsset;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize(Policy = "IsAdministrator")]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class SeriesAssetAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SeriesAssetAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public SeriesAssetAdminController(IConfiguration configuration,
            ILogger<SeriesAssetAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        // displays the specific asset
        public async Task<ActionResult> Detail(Guid id)
        {
            var seriesAssetDto = await _eventHostService
                .GetOne<SeriesAssetDto>($"/seriesassets/{id}");

            // verify object returned contains values
            if (!TryValidateModel(seriesAssetDto, nameof(seriesAssetDto)))
            {
                return RedirectToAction("NotFound", "Home");
            }

            var seriesAssetDetailVM = new SeriesAssetDetailVM
            {
                SeriesAsset = seriesAssetDto,
                ReturnUrl = Request.Headers["Referer"].ToString(),
                SeriesName = await GetSeriesName(seriesAssetDto.SeriesId)
            };

            return View(seriesAssetDetailVM);
        }

        public async Task<ActionResult> Add(Guid seriesId)
        {
            var seriesAssetForUpdateDto = new SeriesAssetForUpdateDto()
            {
                SeriesId = seriesId
            };

            var assetEditViewModel = new SeriesAssetEditVM
            {
                SeriesAsset = seriesAssetForUpdateDto,
                AssetList = await BuildAssetList(),
                SeriesName = await GetSeriesName(seriesId)
            };

            return View(assetEditViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Add(SeriesAssetEditVM seriesAssetEditVM)
        {
            string stringData = JsonConvert.SerializeObject(seriesAssetEditVM.SeriesAsset);

            var seriesAssetDto = await _eventHostService
                .PostOne<SeriesAssetDto>($"/series/{seriesAssetEditVM.SeriesAsset.SeriesId}/assets", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Detail", "SeriesAdmin", new { id = seriesAssetDto.SeriesId });
            }

            var assetEditViewModel = new SeriesAssetEditVM
            {
                SeriesAsset = seriesAssetEditVM.SeriesAsset,
                AssetList = await BuildAssetList(),
                SeriesName = await GetSeriesName(seriesAssetEditVM.SeriesAsset.SeriesId)
            };

            return View(assetEditViewModel);
        }


        public async Task<ActionResult> Delete(Guid id)
        {
            var assetToDelete = await _eventHostService.GetOne<SeriesAssetDto>($"/seriesassets/{id}");

            if (assetToDelete == null)
            {
                return NotFound();
            }

            await _eventHostService.DeleteOne($"/seriesassets/{id}");

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            return RedirectToAction("Detail", "SeriesAdmin", new { id = assetToDelete.SeriesId });
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

        private async Task<string> GetSeriesName(Guid seriesId)
        {
            var seriesDto = await _eventHostService
                .GetOne<SeriesDto>($"/series/{seriesId}");

            return seriesDto.Title;
        }
    }
}