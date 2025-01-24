using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GDC.EventHost.App.Components
{
    public class BlobStorageEventAsset : IEventAssetStorage
    {
        public async Task<string> SaveEventAsset(string fileName, Stream content, IConfiguration? config = null)
        {
            var baseUri = new Uri(config["AzureStorageBaseUri"]);
            var accountName = config["AzureStorageKeys__AccountName"];
            var containerName = config["AzureStorageKeys__ContainerName"];
            var accountKey = config["AzureStorageKeys__AccountKey"];

            var credentials = new StorageCredentials(accountName, accountKey);
            var client = new CloudBlobClient(baseUri, credentials);

            var container = client.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(content);

            return $"{baseUri}{containerName}/{fileName}";
        }
    }
} 
