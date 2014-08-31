using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class ActivityViewModel : SimpleViewModel
    {

        public ActivityViewModel(Uri basePictureUrl, ActivityInfo info, String text)
        {
            PictureUrl = new Uri(basePictureUrl, info.MediaUri);
            MediaUri = info.MediaUri;
            ProfileId = info.ProfileId;
            ActivityType = info.ActivityType;
            Timestamp = info.Timestamp;
            CommentId = info.CommentId;
            Message = info.Message;
            Creator = info.Creator;
            Online = info.Online;
            Text = text;
        }

        public Uri PictureUrl { get { return Get<Uri>(); } set { Set(value); } }

        public Uri MediaUri { get { return Get<Uri>(); } set { Set(value); } }

        public Guid ProfileId { get { return Get<Guid>(); } set { Set(value); } }

        public ActivityType ActivityType { get { return Get<ActivityType>(); } set { Set(value); } }

        public DateTime Timestamp { get { return Get<DateTime>(); } set { Set(value); } }

        public Guid? CommentId { get { return Get<Guid?>(); } set { Set(value); } }

        public String Message { get { return Get<String>(); } set { Set(value); } }

        public String Creator { get { return Get<String>(); } set { Set(value); } }

        public Boolean Online { get { return Get<Boolean>(); } set { Set(value); } }

        public String Text { get { return Get<String>(); } set { Set(value); } }
    }
}
