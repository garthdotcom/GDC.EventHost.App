using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Performance;
using GDC.EventHost.App.ViewModels.Performances;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class PerformancesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PerformancesController> _logger;
        private readonly IEventHostService _eventHostService;

        public PerformancesController(IConfiguration configuration,
            ILogger<PerformancesController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> List(string searchQuery)
        {
            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/performances");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var eventListViewModel = new PerformanceListVM
            {
                PerformanceDetails = await _eventHostService
                .GetMany<PerformanceDetailDto>(uriBuilder.ToString())
            };

            return View(eventListViewModel);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var performanceDetail = await _eventHostService
                .GetOne<PerformanceDetailDto>($"/performances/{id}");

            //// verify object returned contains values
            //if (!TryValidateModel(performanceDetail, nameof(performanceDetail)))
            //{
            //    return NotFound();  // todo - log this
            //}

            var performanceDetailViewModel = new PerformanceDetailVM
            {
                PerformanceDetail = performanceDetail,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(performanceDetailViewModel);
        }

    }
}