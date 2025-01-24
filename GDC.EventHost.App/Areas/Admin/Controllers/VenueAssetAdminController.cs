using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Asset;
using GDC.EventHost.Shared.Venue;
using GDC.EventHost.Shared.VenueAsset;
using GDC.EventHost.App.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class VenueAssetAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VenueAssetAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public VenueAssetAdminController(IConfiguration configuration,
            ILogger<VenueAssetAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        // displays the specific asset
        public async Task<ActionResult> Detail(Guid id)
        {
            var venueAssetDto = await _eventHostService
                .GetOne<VenueAssetDto>($"/venueassets/{id}");

            // verify object returned contains values
            if (!TryValidateModel(venueAssetDto, nameof(venueAssetDto)))
            {
                return NotFound();  // todo - create a friendly not found page
            }

            var venueAssetDetailVM = new VenueAssetDetailVM
            {
                VenueAsset = venueAssetDto,
                ReturnUrl = Request.Headers["Referer"].ToString(),
                VenueName = await GetVenueName(venueAssetDto.VenueId)
            };

            return View(venueAssetDetailVM);
        }

        public async Task<ActionResult> Add(Guid venueId)
        {
            var venueAssetForUpdateDto = new VenueAssetForUpdateDto()
            {
                VenueId = venueId
            };

            var assetEditViewModel = new VenueAssetEditVM
            {
                VenueAsset = venueAssetForUpdateDto,
                AssetList = await BuildAssetList(),
                VenueName = await GetVenueName(venueId)
            };

            return View(assetEditViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Add(VenueAssetEditVM venueAssetEditVM)
        {
            string stringData = JsonConvert.SerializeObject(venueAssetEditVM.VenueAsset);

            var venueAssetDto = await _eventHostService
                .PostOne<VenueAssetDto>($"/venues/{venueAssetEditVM.VenueAsset.VenueId}/assets", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Detail", "VenueAdmin", new { id = venueAssetDto.VenueId });
            }

            var assetEditViewModel = new VenueAssetEditVM
            {
                VenueAsset = venueAssetEditVM.VenueAsset,
                AssetList = await BuildAssetList(),
                VenueName = await GetVenueName(venueAssetEditVM.VenueAsset.VenueId)
            };

            return View(assetEditViewModel);
        }


        public async Task<ActionResult> Delete(Guid id)
        {
            var assetToDelete = await _eventHostService.GetOne<VenueAssetDto>($"/venueassets/{id}");

            if (assetToDelete == null)
            {
                return NotFound();
            }

            await _eventHostService.DeleteOne($"/venueassets/{id}");

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            return RedirectToAction("Detail", "VenueAdmin", new { id = assetToDelete.VenueId });
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

        private async Task<string> GetVenueName(Guid venueId)
        {
            var venueDto = await _eventHostService
                .GetOne<VenueDto>($"/venues/{venueId}");

            return venueDto.Name;
        }
    }
}