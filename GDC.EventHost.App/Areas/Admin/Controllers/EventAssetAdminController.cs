using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Asset;
using GDC.EventHost.Shared.Event;
using GDC.EventHost.Shared.EventAsset;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class EventAssetAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EventAssetAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public EventAssetAdminController(IConfiguration configuration,
            ILogger<EventAssetAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        // displays the specific asset
        public async Task<ActionResult> Detail(Guid id)
        {
            var eventAssetDto = await _eventHostService
                .GetOne<EventAssetDto>($"/eventassets/{id}");

            // verify object returned contains values
            if (!TryValidateModel(eventAssetDto, nameof(eventAssetDto)))
            {
                return NotFound();  // todo - create a friendly not found page
            }

            var eventAssetDetailVM = new EventAssetDetailVM
            {
                EventAsset = eventAssetDto,
                ReturnUrl = Request.Headers["Referer"].ToString(),
                EventTitle = await GetEventTitle(eventAssetDto.EventId)
            };
             
            return View(eventAssetDetailVM);
        }

        public async Task<ActionResult> Add(Guid eventId)
        {
            var eventAssetForUpdateDto = new EventAssetForUpdateDto()
            {
                EventId = eventId
            };

            var assetEditViewModel = new EventAssetEditVM
            {
                EventAsset = eventAssetForUpdateDto,
                //AssetTypeList = BuildAssetTypeList(),
                AssetList = await BuildAssetList(),
                EventTitle = await GetEventTitle(eventId)
            };

            // TODO
            // for some reason my implementation of the enum assetType behaves differently
            // compared to regular entities. no matter what i do, the -- Select One -- option
            // is never selected when the control is first rendered. i know that i can control
            // which one is selected in code by setting a value here for the asp-for value. in this
            // case that is the AssetTypeId, but that is of little help because i don't want 'unknown'
            // to be a valid enum choice in the db.
            // likely something to do with how i have implemented the enum type.
            // i've implemented a cleaner approach in the view using the extension method:
            // Html.GetEnumSelectList<Enums.AssetTypeEnum>() which makes it unnecessary to build out
            // the list here in the controller. but the issue still remains.
            // some guidance here: https://gunnarpeipman.com/aspnet-core-enum-to-select-list/

            return View(assetEditViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Add(EventAssetEditVM eventAssetEditVM) 
        {
            string stringData = JsonConvert.SerializeObject(eventAssetEditVM.EventAsset);

            var eventAssetDto = await _eventHostService
                .PostOne<EventAssetDto>($"/events/{eventAssetEditVM.EventAsset.EventId}/assets", stringData);

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Detail", "EventAdmin", new { id = eventAssetDto.EventId });
            }

            var assetEditViewModel = new EventAssetEditVM
            {
                EventAsset = eventAssetEditVM.EventAsset,
                AssetList = await BuildAssetList(),
                EventTitle = await GetEventTitle(eventAssetEditVM.EventAsset.EventId)
            };

            return View(assetEditViewModel);
        }


        public async Task<ActionResult> Delete(Guid id)
        {
            var assetToDelete = await _eventHostService.GetOne<EventAssetDto>($"/eventassets/{id}");

            if (assetToDelete == null)
            {
                return NotFound();
            }

            await _eventHostService.DeleteOne($"/eventassets/{id}");

            if (_eventHostService.Error)
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            return RedirectToAction("Detail", "EventAdmin", new { id = assetToDelete.EventId });
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

        private async Task<string> GetEventTitle(Guid eventId)
        {
            var eventDto = await _eventHostService
                .GetOne<EventDto>($"/events/{eventId}");

            return eventDto.Title;
        }
    }
}