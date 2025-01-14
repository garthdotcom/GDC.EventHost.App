using GDC.EventHost.App;
using GDC.EventHost.App.ApiServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// debugging
builder.Services.AddProblemDetails();
//IdentityModelEventSource.ShowPII = true;

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<ISeriesApiService, SeriesApiService>();
builder.Services.AddScoped<EnsureAccessTokenFilter>();

builder.Services.AddSingleton(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiUri"]) };
    return client;
});

//JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(o => o.Events.OnSigningOut =
        async e => await e.HttpContext.RevokeRefreshTokenAsync())

    .AddOpenIdConnect(options =>
    {
        options.Authority = builder.Configuration["IdpUri"];
        options.ClientId = "eventhost_web";
        options.ClientSecret = builder.Configuration["WebClientSecret"];
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("eventhost");
        options.Scope.Add("eventhostapi");
        
        options.ResponseType = "code";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;

        //options.ClaimActions.MapUniqueJsonKey("membershipnumber", "membershipnumber");
        //options.ClaimActions.MapUniqueJsonKey("birthdate", "birthdate");
        //options.ClaimActions.MapUniqueJsonKey("role", "role");
        //options.ClaimActions.MapUniqueJsonKey("permission", "permission");

        options.Events = new OpenIdConnectEvents
        {
            OnTokenResponseReceived = r =>
            {
                var accessToken = r.TokenEndpointResponse.AccessToken;
                return Task.CompletedTask;
            }
        };
    });

//if (builder.Environment.IsProduction())
//{
//    builder.Configuration.AddAzureKeyVault(
//        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
//        new DefaultAzureCredential());
//}

builder.Services.AddOpenIdConnectAccessTokenManagement();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
