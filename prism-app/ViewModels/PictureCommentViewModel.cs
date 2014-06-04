using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class PictureCommentViewModel : SimpleViewModel
    {
        public PictureCommentViewModel(string profileUriPattern, CommentInfo item)
        {
            CommentId = item.CommentId;
            MediaUri = item.MediaUri;
            ProfileId = item.ProfileId;
            Creation = item.Creation;
            Comment = item.Comment;
            Creator = item.Creator;
            Online = item.Online;
            ProfilePictureUrl = new Uri(string.Format(profileUriPattern, ProfileId), UriKind.Absolute);

        }
        public Guid CommentId { get { return Get<Guid>(); } set { Set(value); } }
        public Uri MediaUri { get { return Get<Uri>(); } set { Set(value); } }
        public Guid ProfileId { get { return Get<Guid>(); } set { Set(value); } }
        public DateTime Creation { get { return Get<DateTime>(); } set { Set(value); } }
        public string Comment { get { return Get<string>(); } set { Set(value); } }
        public string Creator { get { return Get<string>(); } set { Set(value); } }
        public bool Online { get { return Get<bool>(); } set { Set(value); } }
        public bool Offline { get { return !Online; } }

        public Uri ProfilePictureUrl { get { return Get<Uri>(); } set { Set(value); } }
    }
}
