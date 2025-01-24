namespace GDC.EventHost.App.Components
{
    public interface IEventAssetStorage
    {
        Task<string> SaveEventAsset(string name, Stream content, IConfiguration config = null);
    }
} 
