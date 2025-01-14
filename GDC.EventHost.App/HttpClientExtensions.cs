using IdentityModel.Client;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace GDC.EventHost.App
{
    public static class HttpClientExtensions
    {
        private static string? _accessToken;
        private static IConfiguration? _config;

        private static async Task<string> FetchAccessToken(HttpClient client)
        {
            // this will be called for each access to the api, so some caching is
            // done here. default lifetime is 24h.
            var discoClient = new DiscoveryCache(_config["IdpUri"]);
            var disco = await discoClient.GetAsync();

            var tokenResponse = await client.
                RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "eventhost_api",
                    ClientSecret = _config["ApiClientSecret"],
                    Scope = "eventhostapi"
                });

            _accessToken = tokenResponse.AccessToken;

            return _accessToken;
        }

        private static async Task<string> GetAccessToken(HttpClient client)
        {
            if (_accessToken is not null)
            {
                var tokenObject = new JwtSecurityTokenHandler()
                    .ReadToken(_accessToken);

                if (DateTime.UtcNow.AddMinutes(1) < tokenObject.ValidTo)    // 1 min margin
                {
                    return _accessToken;
                } 
            }

            return await FetchAccessToken(client);
        }

        public static async Task EnsureAccessTokenInHeader(
            this HttpClient client, IConfiguration config)
        {
            _config = config ??
                throw new ArgumentNullException(nameof(config));

            var token = await GetAccessToken(client);

            client.SetBearerToken(token);
        }
    }
}
