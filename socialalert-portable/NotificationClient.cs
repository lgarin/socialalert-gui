using Bravson.Socialalert.Portable.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public interface INotificationService : IDisposable
    {
        void Show(LocalNotification notification);
    }

    public class LocalNotification
    {
        public int Id { get; set; }

        public Color? Color { get; set; }

        public string Icon { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public int? Progress { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Ongoing { get; set; }

        public string BroadcastMessage { get; set; }
    }

    public class NotificationClient : IDisposable
    {
        private const string UploadMessage = "uploadAction";

        private INotificationService service;

        public NotificationClient()
        {
            service = DependencyService.Get<INotificationService>();
            MessagingCenter.Subscribe<NotificationClient, int>(this, UploadMessage, OnUploadAction);
        }

        public void ShowUpload(PendingUpload upload)
        {
            var notification = new LocalNotification() { Id = upload.Id.Value, Title = "Uploading", Body = "Test", Timestamp = upload.Timestamp, Ongoing = true, Color = Color.Blue, Icon = "alarm63.png", BroadcastMessage = UploadMessage };
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
