using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GDC.EventHost.App.Controllers
{
    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly ILogger<MyAccountController> _logger;
        private readonly IConfiguration _config;

        public MyAccountController(ILogger<MyAccountController> logger, 
            IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> LogOut()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var ru = new RequestUrl($"{_config["IdpUri"]}/connect/end_session");

            var url = ru.CreateEndSessionUrl(
                idTokenHint: accessToken,
                postLogoutRedirectUri: $"{_config["WebUri"]}/signout-callback-oidc");

            return SignOut(new AuthenticationProperties { RedirectUri = url },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
