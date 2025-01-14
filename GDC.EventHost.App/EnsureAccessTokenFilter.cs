using Microsoft.AspNetCore.Mvc.Filters;

namespace GDC.EventHost.App
{
    public class EnsureAccessTokenFilter : ActionFilterAttribute
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public EnsureAccessTokenFilter(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            await _httpClient.EnsureAccessTokenInHeader(_config);
            await next();
        }
    }
}
