using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace Socialalert.Services
{
    public interface IImageUploadService
    {
        Task<Uri> PostPictureAsync(IInputStream picture);
    }

    public sealed class ImageUploadService : IImageUploadService, IDisposable
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly Uri serverUri;

        public ImageUploadService(String serverUrl)
        {
            serverUri = new Uri(serverUrl, UriKind.Absolute);
        }

        public async Task<Uri> PostPictureAsync(IInputStream picture)
        {
            return await PostFileAsync(picture, "image/jpeg");
        }

        public async Task<Uri> PostVideoAsync(IInputStream video, string filename)
        {
            if (filename.EndsWith("mov"))
            {
                return await PostFileAsync(video, "video/quicktime");
            }
            else
            {
                return await PostFileAsync(video, "video/mp4");
            }
        }

        private async Task<Uri> PostFileAsync(IInputStream stream, string contentType)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, serverUri))
            {
                request.Content = new HttpStreamContent(stream);
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
