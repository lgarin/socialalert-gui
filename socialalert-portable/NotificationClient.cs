using Bravson.Socialalert.Portable.Model;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public interface INotificationService : IDisposable
    {
        void Show(LocalNotification notification);
    }

    public class LocalNotification
    {
        public LocalNotification(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        public Color? Color { get; set; }

        public string Icon { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public int? ProgressPercentage { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Ongoing { get; set; }

        public string BroadcastMessage { get; set; }
    }

    public class NotificationClient : IDisposable
    {
        private const string UploadMessage = "uploadAction";

        private readonly INotificationService service;
        private readonly ResourceDictionary resources;

        public NotificationClient(ResourceDictionary resources)
        {
            this.resources = resources;
            service = DependencyService.Get<INotificationService>();
            MessagingCenter.Subscribe<NotificationClient, int>(this, UploadMessage, OnUploadAction);
        }

        public void ShowUpload(PendingUpload upload)
        {
            var notification = new LocalNotification(upload.Id.Value) { Title = "Upload media".Translate(resources), Body = upload.State.GetDescription(resources), Color = upload.Color, Timestamp = upload.Timestamp, Ongoing = upload.State == UploadState.Completed, Icon = "alarm63.png", BroadcastMessage = UploadMessage };
            service.Show(notification);
        }

        private void OnUploadAction(NotificationClient client, int notificationId)
        {
            Debug.WriteLine(notificationId.ToString() + " - " + client.GetHashCode());
        }

        public void Dispose()
        {
            service.Dispose();
        }
    }
}
