namespace GDC.EventHost.App.ApiServices
{
    public interface IEventHostService
    {
        bool Error { get; }
        string? Messages { get; }

        Task DeleteOne(string uri);
        Task<IEnumerable<T>> GetMany<T>(string uri);
        Task<T?> GetOne<T>(string uri);
        Task PatchOne(string uri, string stringData);
        Task<T> PostOne<T>(string uri, string stringData);
        Task PutOne(string uri, string stringData);
    }
}
