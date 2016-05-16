using Bravson.Socialalert.Portable.Data;
using Bravson.Socialalert.Portable.Model;
using PCLStorage;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bravson.Socialalert.Portable
{
    public sealed class UploadService : IDisposable
    {
        private volatile bool stop;

        private readonly ConcurrentQueue<PendingUpload> uploadQueue = new ConcurrentQueue<PendingUpload>();

        public async void Run()
        {
            while (!stop)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                PendingUpload upload;
                if (uploadQueue.TryDequeue(out upload))
                {
                    if (await UploadFile(upload) && upload.CanClaim)
                    {
                        await ClaimUpload(upload);
                    }
                }
            }
        }

        private async Task ClaimUpload(PendingUpload upload)
        {
            /*
            try
            {
                App.ServerConnection.InvokeAsync(new ClaimPictureRequest() { PictureUri = upload.Uri, Title = upload.Title, Categories = upload.Category, Tags = upload.Tags})
            }
            catch (Exception)
            {
                upload.State = UploadState.Error;
            }
            App.Notification.ShowUpload(upload);
            */
        }

        private async Task<bool> UploadFile(PendingUpload upload)
        {
            try
            {
                upload.State = UploadState.Uploading;
                App.Notification.ShowUpload(upload);

                using (var stream = await OpenFile(upload.FilePath))
                {
                    var uri = await App.ServerConnection.PostAsync(stream, upload.ContentType, new Progress<int>(p => ShowProgress(upload, p)));
                    upload.Uri = uri.ToString();
                }
                await App.DatabaseConnection.UpsertPendingUpload(upload);
            }
            catch (Exception)
            {
                upload.State = UploadState.Error;
            }
            App.Notification.ShowUpload(upload);
            return upload.State != UploadState.Error;
        }

        private void ShowProgress(PendingUpload upload, int progress)
        {
            App.Notification.ShowUpload(upload, progress);
        }

        private async Task<Stream> OpenFile(string path)
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(path);
            return await file.OpenAsync(FileAccess.Read);
        }

        public async void Upload(MediaFile file)
        {
            PendingUpload upload = new PendingUpload(MediaType.PICTURE, file.Path);
            try
            {
                await App.DatabaseConnection.UpsertPendingUpload(upload);
                uploadQueue.Enqueue(upload);
            }
            catch (Exception)
            {
                upload.State = UploadState.Error;
            }
            App.Notification.ShowUpload(upload);
        }

        public void Dispose()
        {
            stop = true;
        }
    }
}
