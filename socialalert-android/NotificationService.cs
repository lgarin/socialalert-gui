using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bravson.Socialalert.Portable;
using Xamarin.Forms;
using Android.Support.V4.App;
using Xamarin.Forms.Platform.Android;
using Bravson.Socialalert.Portable.Util;
using System.Threading;
using System.IO;

[assembly: Dependency(typeof(Bravson.Socialalert.Android.NotificationService))]
namespace Bravson.Socialalert.Android
{
    public class NotificationService : INotificationService
    {
        public const string ExtraNotificationId = "ID";
        public const string ExtraBroadcastMessage = "Message";

        public void Show(LocalNotification notification)
        {
            var context = MainApplication.Context;
            var builder = new NotificationCompat.Builder(context);
            builder.SetContentTitle(notification.Title);
            builder.SetContentText(notification.Body);
            builder.SetOngoing(notification.Ongoing);
            //builder.SetWhen(notification.Timestamp.GetEpochMillis());
            if (notification.Progress != null)
            {
                builder.SetProgress(100, notification.Progress.Value, false);
            }
            if (notification.Color != null)
            {
                builder.SetColor(notification.Color.Value.ToAndroid().ToArgb());
            }
            
            if (notification.Icon != null)
            {
                builder.SetSmallIcon(context.Resources.GetIdentifier(Path.GetFileNameWithoutExtension(notification.Icon), "drawable", context.PackageName));
            }
            else
            {
                builder.SetSmallIcon(Resource.Drawable.alarm63);
            }            

            if (notification.BroadcastMessage != null) { 
                var intent = new Intent(context, typeof(NotificationActionService)).PutExtra(ExtraNotificationId, notification.Id).PutExtra(ExtraBroadcastMessage, notification.BroadcastMessage);
                builder.SetContentIntent(PendingIntent.GetService(context, 0, intent, PendingIntentFlags.UpdateCurrent));
            }

            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify(notification.Id, builder.Build());
        }

        public void Dispose()
        {
            var context = MainApplication.Context;
            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.CancelAll();
        }
    }

    [Service]
    public class NotificationActionService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            int id = intent.GetIntExtra(Android.NotificationService.ExtraNotificationId, 0);
            string message = intent.GetStringExtra(Android.NotificationService.ExtraBroadcastMessage);
            if (id > 0 && message != null) {
                MessagingCenter.Send(DependencyService.Get<INotificationService>(), message, id);
            }

            return StartCommandResult.NotSticky;
        }
    }
}