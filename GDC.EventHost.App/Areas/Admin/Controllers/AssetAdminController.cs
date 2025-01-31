using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.Models;
using GDC.EventHost.Shared.Asset;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using GDC.EventHost.App.Components;
using GDC.EventHost.App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize(Policy = "IsAdministrator")]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class AssetAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AssetAdminController> _logger;
        private readonly IEventHostService _eventHostService;
        private readonly IEventAssetStorage _assetStorage;

        public AssetAdminController(IConfiguration configuration,
            ILogger<AssetAdminController> logger,
            IEventHostService eventHostService,
            IEventAssetStorage assetStorage)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
            _assetStorage = assetStorage;
        }

        public async Task<ActionResult> Index(string searchQuery, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchQuery;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["AssetNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "asset_name_desc" : "";
            ViewData["DescriptionSortParm"] = sortOrder == "Description" ? "description_desc" : "Description";
            ViewData["FileNameSortParm"] = sortOrder == "FileName" ? "file_name_desc" : "FileName";

            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/assets");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var assetDtos = await _eventHostService.GetMany<AssetDetailDto>(uriBuilder.ToString());

            switch (sortOrder)
            {
                case "asset_name_desc":
                    assetDtos = assetDtos.OrderByDescending(s => s.Name);
                    break;
                case "Description":
                    assetDtos = assetDtos.OrderBy(s => s.Description);
                    break;
                case "description_desc":
                    assetDtos = assetDtos.OrderByDescending(s => s.Description);
                    break;
                case "FileName":
                    assetDtos = assetDtos.OrderBy(s => s.Uri);
                    break;
                case "file_name_desc":
                    assetDtos = assetDtos.OrderByDescending(s => s.Uri);
                    break;
                default:
                    assetDtos = assetDtos.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;

            var assetList = new PaginatedList<AssetDetailDto>(assetDtos.AsQueryable(),
                pageNumber ?? 1, pageSize);

            var assetListViewModel = new AssetListVM
            {
                AssetList = assetList
            };

            return View(assetListViewModel);
        }


        public async Task<ActionResult> Detail(Guid id)
        {
            var assetDetailDto = await _eventHostService.GetOne<AssetDetailDto>($"/assets/{id}");

            // verify object returned contains values
            if (!TryValidateModel(assetDetailDto, nameof(assetDetailDto)))
            {
                return RedirectToAction("NotFound", "Home");
            }

            var assetDetailViewModel = new AssetDetailVM
            {
                Asset = assetDetailDto,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(assetDetailViewModel);
        }


        public async Task<ActionResult> Edit(Guid id)
        {
            var assetForUpdateDto = await _eventHostService.GetOne<AssetForUpdateDto>($"/assets/{id}");

            var assetEditViewModel = new AssetEditVM
            {
                Asset = assetForUpdateDto,
                AssetTypeList = BuildAssetTypeList()
            };

            return View(assetEditViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(AssetEditVM assetVM)
        {
            if (assetVM.FileUpload != null)
            {
                // upload the selected file and set the value
                var uploadedFile = await UploadFile(assetVM.FileUpload);

                if (uploadedFile.UploadWasSuccessful())
                {
                    assetVM.Asset.Uri = uploadedFile.PathToFile;
                }
                else
                {
                    ModelState.AddModelError("", uploadedFile.ErrorMessages);
                }
            }

            if (!string.IsNullOrEmpty(assetVM.Asset.Uri))
            {
                string stringData = JsonConvert.SerializeObject(assetVM.Asset);

                if (assetVM.Asset.Id == Guid.Empty)
                {
                    var assetDetailDto = await _eventHostService.PostOne<AssetDetailDto>("/assets", stringData);

                    if (_eventHostService.Error)
                    {
                        ModelState.AddModelError("", _eventHostService.Messages);
                    }

                    if (ModelState.IsValid)
                    {
                        return RedirectToAction("Detail", new { id = assetDetailDto.Id });
                    }
                }
                else
                {
                    await _eventHostService.PutOne($"/assets/{assetVM.Asset.Id}", stringData);

                    if (_eventHostService.Error)
                    {
                        ModelState.AddModelError("", _eventHostService.Messages);
                    }

                    if (ModelState.IsValid)
                    {
                        return RedirectToAction("Detail", new { id = assetVM.Asset.Id });
                    }
                }
            }

            var assetEditViewModel = new AssetEditVM
            {
                Asset = assetVM.Asset,
                AssetTypeList = BuildAssetTypeList()
            };

            return View(assetEditViewModel);
        }


        public async Task<ActionResult> Delete(Guid id)
        {
            var assetToDelete = await _eventHostService.GetOne<AssetDto>($"/assets/{id}");

            if (assetToDelete == null)
            {
                return NotFound();
            }

            await _eventHostService.DeleteOne($"/assets/{id}");

            if (_eventHostService.Error) 
            {
                ModelState.AddModelError("", _eventHostService.Messages);
            }

            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<SelectListItem> BuildAssetTypeList()
        {
            var theList = new List<SelectListItem>();

            var enumValues = Enum.GetValues(typeof(AssetTypeEnum));
            var enumNames = Enum.GetNames(typeof(AssetTypeEnum));

            var nameCount = 0;
            foreach (var value in enumValues)
            {
                theList.Add(new SelectListItem { Value = value.ToString(), Text = enumNames[nameCount] });
                nameCount++;
            }

            return theList;
        }

        private async Task<UploadFileInfo> UploadFile(IFormFile fileToUpload)
        {
            var result = new UploadFileInfo();

            var errorMessageBuilder = new StringBuilder();

            try
            {
                // obtain configuration settings

                if (!Int32.TryParse(_config["MaxFileUploadSizeInBytes"], out int maxSize))
                {
                    errorMessageBuilder.Append("Configuration setting 'MaxFileUploadSizeInBytes' is absent or invalid.");
                }

                using var reader = fileToUpload.OpenReadStream();

                // checking size
                if (fileToUpload.Length > maxSize)
                {
                    var maxSizeInKilobytes = Math.Truncate(maxSize / 1024M);
                    var enteredFileInKilobytes = Math.Truncate(fileToUpload.Length / 1024M);

                    errorMessageBuilder.Append($"Files are limited to {maxSize}K. " +
                        $"Your file is {fileToUpload.Length.ToString()}K.");
                }

                // checking type
                var provider = new FileExtensionContentTypeProvider();

                if (provider.TryGetContentType(fileToUpload.FileName, out string contentType))
                {
                    var allowedTypes = _config["AllowedFileUploadFileTypes"].Split(',');
                    if (!allowedTypes.Contains(contentType))
                    {
                        errorMessageBuilder.Append(
                        $"Uploads are limited to the types: {_config["AllowedFileUploadFileTypes"]} bytes. " +
                        $"Your file is of type '{contentType}'.");
                    }
                }

                if (errorMessageBuilder.Length == 0)
                {
                    result.PathToFile = await _assetStorage.SaveEventAsset(fileToUpload.FileName, reader, _config);
                }
            }
            catch (Exception ex)
            {
                errorMessageBuilder.Append(ex.Message);
            }

            result.ErrorMessages = errorMessageBuilder.ToString();
            return result;
        }

    }
}