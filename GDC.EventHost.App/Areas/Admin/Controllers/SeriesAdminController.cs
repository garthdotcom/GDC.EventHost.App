using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Series;
using GDC.EventHost.App.Areas.Admin.ViewModels;
using GDC.EventHost.App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace GDC.EventHost.App.Areas.Admin.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    [Area("Admin")]
    public class SeriesAdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SeriesAdminController> _logger;
        private readonly IEventHostService _eventHostService;

        public SeriesAdminController(IConfiguration configuration,
            ILogger<SeriesAdminController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> Index(string searchQuery, string sortOrder, int? pageNumber) 
        {
            ViewData["CurrentFilter"] = searchQuery;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DescriptionSortParm"] = sortOrder == "Description" ? "description_desc" : "Description";

            var uriBuilder = new StringBuilder();
            uriBuilder.Append("/series");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                uriBuilder.Append("?searchQuery=");
                uriBuilder.Append(WebUtility.UrlEncode(searchQuery));
            }

            var seriesDtos = await _eventHostService.GetMany<SeriesDetailDto>(uriBuilder.ToString());

            switch (sortOrder)
            {
                case "name_desc":
                    seriesDtos = seriesDtos.OrderByDescending(s => s.Title);
                    break;
                case "Description":
                    seriesDtos = seriesDtos.OrderBy(s => s.Description);
                    break;
                case "description_desc":
                    seriesDtos = seriesDtos.OrderByDescending(s => s.Description);
                    break;
                default:
                    seriesDtos = seriesDtos.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 10;

            var seriesList = new PaginatedList<SeriesDetailDto>(seriesDtos.AsQueryable(),
                pageNumber ?? 1, pageSize);

            var seriesListViewModel = new SeriesListVM
            {
                SeriesList = seriesList
            };

            return View(seriesListViewModel);
        }

        public async Task<ActionResult> Detail(Guid id)
        {
            var seriesDetail = await _eventHostService.GetOne<SeriesDetailDto>($"/series/{id}");

            // verify object returned contains values
            if (!TryValidateModel(seriesDetail, nameof(seriesDetail)))
            {
                return NotFound();  // todo - create a friendly not found page
            }

            var seriesDetailViewModel = new SeriesDetailVM
            {
                Series = seriesDetail,
                ReturnUrl = Request.Headers["Referer"].ToString()
            };

            return View(seriesDetailViewModel);
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var series = await _eventHostService.GetOne<SeriesForUpdateDto>($"/series/{id}");

            var seriesEditViewModel = new SeriesEditVM
            {
                Series = series
            };

            if (id == Guid.Empty)
            {
                // on create set the date fields
                seriesEditViewModel.Series.StartDate = DateTime.Now.WithoutSeconds();           //GetDateWithoutSeconds(DateTime.Now);
                seriesEditViewModel.Series.EndDate = DateTime.Now.AddDays(1).WithoutSeconds();  //GetDateWithoutSeconds(DateTime.Now.AddDays(1));
            }

            return View(seriesEditViewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Edit(SeriesForUpdateDto series)
        {
            string stringData = JsonConvert.SerializeObject(series);

            if (series.Id == Guid.Empty)
            {
                var newSeries = await _eventHostService.PostOne<SeriesForUpdateDto>("/series", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = newSeries.Id });
                }
            }
            else
            {
                await _eventHostService.PutOne($"/series/{series.Id}", stringData);

                if (_eventHostService.Error)
                {
                    ModelState.AddModelError("", _eventHostService.Messages);
                }

                if (ModelState.IsValid)
                {
                    return RedirectToAction("Detail", new { id = series.Id });
                }
            }

            var seriesEditViewModel = new SeriesEditVM
            {
                Series = series
            };

            return View(seriesEditViewModel);
        }
    }
}