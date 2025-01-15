using GDC.EventHost.App.DTOs;

namespace GDC.EventHost.App.ApiServices
{
    public interface ISeriesApiService
    {
        Task<IEnumerable<SeriesDetailDto>?> GetAll();
        Task<SeriesDetailDto?> GetById(Guid id);
    }
}
