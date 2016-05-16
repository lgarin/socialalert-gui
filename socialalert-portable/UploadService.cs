using Bravson.Socialalert.Portable.Data;
using Bravson.Socialalert.Portable.Model;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bravson.Socialalert.Portable
{
    public sealed class UploadService : IDisposable
    {
        private volatile bool stop;

        private readonly ConcurrentQueue<PendingUpload> queue = new ConcurrentQueue<PendingUpload>();

        public async void Run()
        {
            while (!stop)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                PendingUpload upload;
                if (queue.TryPeek(out upload))
                {
                    if (upload.State == UploadState.Pending)
                    {
                        //await App.ServerConnection.PostAsync();
                    }
                }
            }
        }

        public async void Upload(MediaFile file)
        {
            PendingUpload upload = new PendingUpload(MediaType.PICTURE, file.Path);
            await App.DatabaseConnection.UpsertPendingUpload(upload);

            App.Notification.ShowUpload(upload);
            
        }

        public void Dispose()
        {
            stop = true;
        }
    }
}
