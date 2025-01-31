using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.App.ViewModels;
using GDC.EventHost.Shared.Series;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HomeController> _logger;
        private readonly IEventHostService _eventHostService;

        public HomeController(IConfiguration configuration,
            ILogger<HomeController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> Index()
        {
            var homeViewModel = new HomeViewModel
            {
                SeriesList = await _eventHostService
                    .GetMany<SeriesDetailDto>("/series")
            };

            return View(homeViewModel);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }
    }
}