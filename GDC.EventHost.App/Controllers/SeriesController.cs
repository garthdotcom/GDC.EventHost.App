using GDC.EventHost.App.ApiServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class SeriesController : Controller
    {
        private readonly ISeriesApiService _apiService;

        public SeriesController(ISeriesApiService service)
        {
            _apiService = service;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Series List";
            return View(await _apiService.GetAll());
        }
    }
}
