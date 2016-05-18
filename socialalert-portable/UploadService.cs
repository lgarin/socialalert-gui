using Bravson.Socialalert.Portable.Data;
using Bravson.Socialalert.Portable.Model;
using PCLStorage;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
            foreach (PendingUpload upload in await App.DatabaseConnection.FetchAllPendingUploads())
            {
                upload.DetermineState();
                if (upload.State == UploadState.Pending)
                {
                    if (await FileExists(upload.FilePath))
                    {
                        uploadQueue.Enqueue(upload);
                    }
                    else
                    {
                        await App.DatabaseConnection.DeletePendingUpload(upload);
                    }
                }
                else if (upload.State == UploadState.Claiming)
                {
                    await ClaimUpload(upload);
                }
                App.Notification.ShowUpload(upload);
            }

            while (!stop)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                PendingUpload upload;
                if (uploadQueue.TryDequeue(out upload))
                {
                    await UploadFile(upload);
                    if (upload.CanClaim)
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

        private async Task UploadFile(PendingUpload upload)
        {
            try
            {
                upload.State = UploadState.Uploading;
                App.Notification.ShowUpload(upload);

                using (var stream = await OpenFile(upload.FilePath))
                {
                    var uri = await App.ServerConnection.PostAsync(stream, upload.ContentType, new Progress<int>(p => ShowProgress(upload, p)));
                    upload.Uri = uri?.ToString();
                }
                if (upload.Uri != null)
                {
                    await App.DatabaseConnection.UpsertPendingUpload(upload);
                    await DeleteFile(upload.FilePath);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                upload.State = UploadState.Error;
            }
            App.Notification.ShowUpload(upload);
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

        private async Task DeleteFile(string path)
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(path);
            await file.DeleteAsync();
        }

        private async Task<bool> FileExists(string path)
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(path);
            return file != null;
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
