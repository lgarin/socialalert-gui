using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class PictureViewModel : SimpleViewModel
    {

        public PictureViewModel(Uri basePicutreUrl, PictureInfo picture)
        {
            PictureUri = picture.PictureUri;
            Title = picture.Title;
            Description = picture.Description;
            Creation = picture.Creation;
            Creator = picture.Creator;
            ImageUrl = new Uri(basePicutreUrl, picture.PictureUri);
        }

        public Uri ImageUrl { get { return Get<Uri>(); } set { Set(value); } }
        public Uri PictureUri { get { return Get<Uri>(); } set { Set(value); } }
        public string Title { get { return Get<string>(); } set { Set(value); } }
        public string Description { get { return Get<string>(); } set { Set(value); } }
        public Guid ProfileId { get { return Get<Guid>(); } set { Set(value); } }
        public DateTime Creation { get { return Get<DateTime>(); } set { Set(value); } }
        public DateTime LastUpdate { get { return Get<DateTime>(); } set { Set(value); } }
        public DateTime? PictureTimestamp { get { return Get<DateTime?>(); } set { Set(value); } }
        public int PictureWidth { get { return Get<int>(); } set { Set(value); } }
        public int PictureHeight { get { return Get<int>(); } set { Set(value); } }
        public double? PictureLongitude { get { return Get<double?>(); } set { Set(value); } }
        public double? PictureLatitude { get { return Get<double?>(); } set { Set(value); } }
        public string Locality { get { return Get<string>(); } set { Set(value); } }
        public string Country { get { return Get<string>(); } set { Set(value); } }
        public string CameraMaker { get { return Get<string>(); } set { Set(value); } }
        public string CameraModel { get { return Get<string>(); } set { Set(value); } }
        public int HitCount { get { return Get<int>(); } set { Set(value); } }
        public int LikeCount { get { return Get<int>(); } set { Set(value); } }
        public int DislikeCount { get { return Get<int>(); } set { Set(value); } }
        public int CommentCount { get { return Get<int>(); } set { Set(value); } }
        public ObservableCollection<string> Categories = new ObservableCollection<string>();
        public ObservableCollection<string> Tags = new ObservableCollection<string>();
        public string UserApprovalModifier { get { return Get<string>(); } set { Set(value); } }
        public string Creator { get { return Get<string>(); } set { Set(value); } }
        public bool Online { get { return Get<bool>(); } set { Set(value); } }
    }
}
