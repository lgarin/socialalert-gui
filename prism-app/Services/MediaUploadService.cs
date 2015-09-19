using Bing.Maps;
using Socialalert.Models;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;

namespace Socialalert.Services
{
    public interface IMediaUploadService
    {
        Task<MediaUploadInfo> PostPictureAsync(StorageFile file);

        Task<MediaUploadInfo> PostVideoAsync(StorageFile file);
    }

    public class MediaUploadInfo
    {
        public Uri Uri;
        public GeoAddress Location;
        public string CameraMaker;
        public string CameraModel;
    }

   

    public sealed class MediaUploadService : IMediaUploadService, IDisposable
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly Uri serverUri;

        public MediaUploadService(String serverUrl)
        {
            serverUri = new Uri(serverUrl, UriKind.Absolute);
        }

        public async Task<MediaUploadInfo> PostPictureAsync(StorageFile file)
        {
            return await PostFileAsync(file, "image/jpeg");
        }

        public async Task<MediaUploadInfo> PostVideoAsync(StorageFile file)
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

        private async Task<MediaUploadInfo> PostFileAsync(StorageFile file, string contentType)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, serverUri))
            {
                request.Content = new HttpStreamContent(await file.OpenReadAsync());
                request.Content.Headers.Add("Content-Type", contentType);
                using (var response = await httpClient.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    return new MediaUploadInfo()
                    {
                        Uri = new Uri(FindHeader(response, "Location"), UriKind.Relative),
                        CameraMaker = FindHeader(response, "CameraMaker"),
                        CameraModel = FindHeader(response, "CameraModel"),
                        Location = MakeLocation(FindHeader(response, "Latitude"), FindHeader(response, "Longitude"))
                    };
                }
            }
        }

        private static string FindHeader(HttpResponseMessage response, string key)
        {
            string value;
            return response.Headers.TryGetValue(key, out value) ? value : null;
        }

        private static GeoAddress MakeLocation(string latitudeString, string longitudeString)
        {
            double temp;
            GeoAddress result = new GeoAddress();
            if (latitudeString != null && double.TryParse(latitudeString, out temp))
            {
                result.Latitude = temp;
            }
            if (longitudeString != null && double.TryParse(longitudeString, out temp))
            {
                result.Longitude = temp;
            }
            return result;
        }

        public void Dispose()
        {
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
