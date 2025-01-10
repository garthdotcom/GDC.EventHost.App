using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.Authority = builder.Configuration["AuthorityUri"];

        options.ClientId = "eventhost_web";
        options.ClientSecret = builder.Configuration["ClientSecret"];

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("eventhost");
        options.Scope.Add("eventhostapi");
        options.SaveTokens = true;
        options.ResponseType = "code";
        options.GetClaimsFromUserInfoEndpoint = true;

        options.ClaimActions.MapUniqueJsonKey("membershipnumber", "membershipnumber");
        options.ClaimActions.MapUniqueJsonKey("birthdate", "birthdate");
        options.ClaimActions.MapUniqueJsonKey("role", "role");
        options.ClaimActions.MapUniqueJsonKey("permission", "permission");

        options.Events = new OpenIdConnectEvents
        {
            OnTokenResponseReceived = r =>
            {
                var idToken = r.TokenEndpointResponse.IdToken;
                return Task.CompletedTask;
            }
        };

    });

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}


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
