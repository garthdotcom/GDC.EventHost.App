using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Controllers
{
    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly ILogger<MyAccountController> _logger;

        public MyAccountController(ILogger<MyAccountController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
