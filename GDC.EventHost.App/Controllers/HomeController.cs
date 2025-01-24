using GDC.EventHost.App;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Controllers
{
    [ServiceFilter(typeof(EnsureAccessTokenFilter))]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var test = User.Identity;
            var claims = User.Claims;
            var asfd = User.IsInRole("Administrator");

            return View();
        }
    }
}