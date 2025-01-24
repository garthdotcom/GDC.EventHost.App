namespace GDC.EventHost.App.Components
{
    public class FileSystemEventAsset : IEventAssetStorage
    {
        private readonly IWebHostEnvironment environment;

        public FileSystemEventAsset(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        public async Task<string> SaveEventAsset(string name, Stream content, IConfiguration config = null)
        {
            var filePath = Path.Combine(environment.WebRootPath, "img", name);

            using (var writer = File.OpenWrite(filePath))
            {
                await content.CopyToAsync(writer);

                return $"~/img/{name}";
            }
        }
    }
}
