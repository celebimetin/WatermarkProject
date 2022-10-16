using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageLibrary
{
    public enum EContainerName
    {
        pictures,
        watermarkpictures,
        pdf,
        logs
    }
    public interface IBlobStorage
    {
        public string BlobUrl { get; }

        Task UploadAsync(Stream fileStream, string fileName, EContainerName econtainerName);
        Task<Stream> DownloadAsync(string fileName, EContainerName econtainerName);
        Task DeleteAsync(string fileName, EContainerName econtainerName);
        Task SetLogAsync(string text, string fileName);
        Task<List<string>> GetLogAsync(string fileName);
        List<string> GetNames(EContainerName econtainerName);
    }
}