using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using GDC.EventHost.Shared.Order;
using GDC.EventHost.App.ViewModels.Members;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class MembersController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MembersController> _logger;
        private readonly IEventHostService _eventHostService;

        public MembersController(IConfiguration configuration,
            ILogger<MembersController> logger,
            IEventHostService eventHostService)
        {
            _config = configuration;
            _logger = logger;
            _eventHostService = eventHostService;
        }

        public async Task<ActionResult> OrderList(Guid id)
        {
            var memberOrderListViewModel = new MemberOrderListVM
            {
                Orders = await _eventHostService
                .GetMany<OrderDto>($"/members/{id}/orders")
            };

            return View(memberOrderListViewModel);
        }
    }
}