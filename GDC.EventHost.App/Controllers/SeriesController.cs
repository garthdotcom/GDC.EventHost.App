using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Series;
using GDC.EventHost.App.ViewModels.Series;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class SeriesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SeriesController> _logger;
        private readonly IEventHostService _eventHostService;

        public SeriesController(IConfiguration configuration,
            ILogger<SeriesController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> List(string searchQuery)
        {
            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/series");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var seriesListViewModel = new SeriesListVM
            {
                SeriesList = await _eventHostService
                .GetMany<SeriesDetailDto>(uriBuilder.ToString())
            };

            return View(seriesListViewModel);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var seriesDetail = await _eventHostService
                .GetOne<SeriesDetailDto>($"/series/{id}");

            //// verify object returned contains values
            //if (!TryValidateModel(seriesDetail, nameof(seriesDetail)))
            //{
            //    return NotFound();  // todo - log this
            //}

            var seriesDetailViewModel = new SeriesDetailVM
            {
                Series = seriesDetail,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(seriesDetailViewModel);
        }

    }
}