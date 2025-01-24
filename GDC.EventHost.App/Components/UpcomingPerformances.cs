using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Performance;
using GDC.EventHost.App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Components
{
    public class UpcomingPerformances : ViewComponent
    {
        private readonly IEventHostService _eventHostService;

        public UpcomingPerformances(IConfiguration _config, 
            IEventHostService eventHostService)
        {
            _eventHostService = eventHostService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var performanceList = await _eventHostService.GetMany<PerformanceDetailDto>($"/performances");

            var filteredList = performanceList
                .Where(e => e.Date > DateTime.Now)
                .OrderBy(e => e.Date)
                .Take(4)
                .ToList();

            var viewModel = new UpcomingPerformancesViewModel
            {
                PerformanceList = filteredList
            };

            return View(viewModel);
        }
    }
}
