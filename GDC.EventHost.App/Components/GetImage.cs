using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.ViewModels;
using GDC.EventHost.Shared.EventAsset;
using GDC.EventHost.Shared.PerformanceAsset;
using GDC.EventHost.Shared.SeriesAsset;
using GDC.EventHost.Shared.VenueAsset;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static GDC.EventHost.Shared.Enums;

namespace GDC.EventHost.App.Components
{
    public class GetImage : ViewComponent
    {
        private readonly IEventHostService _eventHostService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        public GetImage(IConfiguration config, IEventHostService eventHostService, IWebHostEnvironment environment)
        {
            _eventHostService = eventHostService;
            _config = config;
            _environment = environment;
        }

        // based on the id and type of entity and asset, finds the url in the data store and returns it
        // if not found, returns a placeholder image stored in the file system.
        public async Task<IViewComponentResult> InvokeAsync(Guid entityId, string entityType, AssetTypeEnum assetType, 
            int height = 0, int width = 0)
        {
            var viewModel = new GetImageViewModel()
            {
                AssetType = assetType,
                Height = height,
                Width = width
            };

            var assetTypeValue = assetType.ToString();

            switch (entityType)
            {
                case "Series":
                    var seriesAssets = await _eventHostService
                        .GetMany<SeriesAssetDto>($"/series/{entityId}/assets?assetTypeName={assetTypeValue}");
                    var firstSeriesAsset = seriesAssets
                        .Where(e => e.AssetTypeId == assetType)
                        .OrderBy(e => e.OrdinalValue)
                        .FirstOrDefault();
                    if (firstSeriesAsset != null)
                    {
                        viewModel.Name = firstSeriesAsset.Name;
                        viewModel.Uri = firstSeriesAsset.Uri;
                    }
                    break;

                case "Event":
                    var eventAssets = await _eventHostService
                        .GetMany<EventAssetDto>($"/events/{entityId}/assets?assetTypeName={assetTypeValue}");
                    var firstEventAsset = eventAssets
                        .Where(e => e.AssetTypeId == assetType)
                        .OrderBy(e => e.OrdinalValue)
                        .FirstOrDefault();
                    if (firstEventAsset != null)
                    {
                        viewModel.Name = firstEventAsset.Name;
                        viewModel.Uri = firstEventAsset.Uri;
                    }
                    break;

                case "Performance":
                    var perfAssets = await _eventHostService
                        .GetMany<PerformanceAssetDto>($"/performances/{entityId}/assets?assetTypeName={assetTypeValue}");
                    var firstPerfAsset = perfAssets
                        .Where(e => e.AssetTypeId == assetType)
                        .OrderBy(e => e.OrdinalValue)
                        .FirstOrDefault();
                    if (firstPerfAsset != null)
                    {
                        viewModel.Name = firstPerfAsset.Name;
                        viewModel.Uri = firstPerfAsset.Uri;
                    }
                    break;

                case "Venue":
                    var venueAssets = await _eventHostService
                        .GetMany<VenueAssetDto>($"/venues/{entityId}/assets?assetTypeName={assetTypeValue}");
                    var firstVenueAsset = venueAssets
                        .Where(e => e.AssetTypeId == assetType)
                        .OrderBy(e => e.OrdinalValue)
                        .FirstOrDefault();
                    if (firstVenueAsset != null)
                    {
                        viewModel.Name = firstVenueAsset.Name;
                        viewModel.Uri = firstVenueAsset.Uri;
                    }
                    break;
            }

            // test whether image exists at the url

            bool imageExists = false;
            var localImage = false;

            if (!string.IsNullOrEmpty(viewModel.Uri))
            {
                localImage = !viewModel.Uri.Contains("http");

                if (localImage)
                {
                    var filePath = Path.Combine(_environment.WebRootPath, viewModel.Uri.Replace("/", "\\"));
                    imageExists = File.Exists(filePath);
                }
                else
                {
                    HttpWebResponse response = null;
                    var request = (HttpWebRequest)WebRequest.Create(viewModel.Uri);
                    request.Method = "HEAD";
                    request.Timeout = 5000; // milliseconds
                    request.AllowAutoRedirect = false;

                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        imageExists = response.StatusCode == HttpStatusCode.OK;
                    }
                    catch
                    {
                        imageExists = false;
                    }
                    finally
                    {
                        if (response != null)
                        {
                            response.Close();
                        }
                    }
                }
            }

            // if the image does not exist, replace the Uri with the
            // path to a placeholder file located on the file system

            if (!imageExists)
            {
                var fileName = string.Empty;

                switch (assetType)
                {
                    case AssetTypeEnum.LargeImage:
                        fileName = "placeholder-large.jpg";
                        break;
                    case AssetTypeEnum.MediumImage:
                        fileName = "placeholder-medium.jpg";
                        break;
                    case AssetTypeEnum.SmallImage:
                        fileName = "placeholder-small.jpg";
                        break;
                    case AssetTypeEnum.TinyImage:
                        fileName = "placeholder-tiny.jpg";
                        break;
                    case AssetTypeEnum.Avatar:
                        fileName = "placeholder-avatar.jpg";
                        break;
                    default:
                        fileName = "placeholder-large.jpg";
                        break;
                }

                viewModel.Name = "Placeholder";
                viewModel.Uri = $"~/img/{fileName}";
            }
            else
            {
                if (localImage)
                {
                    viewModel.Uri = Path.Combine("~/", viewModel.Uri);
                }
            }

            return View(viewModel);
        }
    }
}
