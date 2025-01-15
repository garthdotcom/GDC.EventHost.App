using GDC.EventHost.App.DTOs;

namespace GDC.EventHost.App.ApiServices
{
    public class SeriesApiService : ISeriesApiService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public SeriesApiService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<IEnumerable<SeriesDetailDto>?> GetAll()
        {
            return await _client
                .GetFromJsonAsync<IEnumerable<SeriesDetailDto>>($"{_config["ApiUri"]}/api/series");
        }

        public Task<SeriesDetailDto?> GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
