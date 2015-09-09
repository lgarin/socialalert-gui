using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace Socialalert.Services
{
    public interface IImageUploadService
    {
        Task<Uri> PostPictureAsync(StorageFile file);

        Task<Uri> PostVideoAsync(StorageFile file);
    }

    public sealed class ImageUploadService : IImageUploadService, IDisposable
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly Uri serverUri;

        public ImageUploadService(String serverUrl)
        {
            serverUri = new Uri(serverUrl, UriKind.Absolute);
        }

        public async Task<Uri> PostPictureAsync(StorageFile file)
        {
            return await PostFileAsync(file, "image/jpeg");
        }

        public async Task<Uri> PostVideoAsync(StorageFile file)
        {
            if (file.Name.EndsWith("mov"))
            {
                return await PostFileAsync(file, "video/quicktime");
            }
            else
            {
                return await PostFileAsync(file, "video/mp4");
            }
        }

        private async Task<Uri> PostFileAsync(StorageFile file, string contentType)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, serverUri))
            {
                request.Content = new HttpStreamContent(await file.OpenReadAsync());
                request.Content.Headers.Add("Content-Type", contentType);
                using (var response = await httpClient.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    var resultString = response.EnsureSuccessStatusCode().Headers["location"];
                    return new Uri(resultString, UriKind.Relative);
                }
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
