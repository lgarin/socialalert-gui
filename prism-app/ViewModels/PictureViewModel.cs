using Bing.Maps;
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
    public sealed class PictureViewModel : SimpleViewModel
    {
        public PictureViewModel(Uri basePicutreUrl, PictureInfo picture)
        {
            PictureUri = picture.PictureUri;
            ImageUrl = new Uri(basePicutreUrl, picture.PictureUri);
            UpdateWith(picture);
        }

        public void UpdateWith(PictureInfo picture)
        {
            Title = picture.Title;
            Description = picture.Description;
            ProfileId = picture.ProfileId;
            Creation = picture.Creation;
            LastUpdate = picture.LastUpdate;
            PictureTimestamp = picture.PictureTimestamp;
            PictureWidth = picture.PictureWidth;
            PictureHeight = picture.PictureHeight;
            PictureLongitude = picture.PictureLongitude;
            PictureLatitude = picture.PictureLatitude;
            Locality = picture.Locality;
            Country = picture.Country;
            CameraMaker = picture.CameraMaker;
            CameraModel = picture.CameraModel;
            HitCount = picture.HitCount;
            LikeCount = picture.LikeCount;
            DislikeCount = picture.DislikeCount;
            CommentCount = picture.CommentCount;
            UserApprovalModifier = picture.UserApprovalModifier;

            foreach (var category in picture.Categories)
            {
                Categories.Add(category);
            }

            foreach (var tag in picture.Tags)
            {
                Tags.Add(tag);
            }

            Creator = picture.Creator;
            Online = picture.Online;
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
        public UserApprovalModifier? UserApprovalModifier { get { return Get<UserApprovalModifier?>(); } set { Set(value); } }
        public string Creator { get { return Get<string>(); } set { Set(value); } }
        public bool Online { get { return Get<bool>(); } set { Set(value); } }
        public bool HasGeoLocation { get { return PictureLatitude.HasValue && PictureLongitude.HasValue; } }
        public Location GeoLocation { get { return HasGeoLocation ? new Location(PictureLatitude.Value, PictureLongitude.Value) : null; } }
        public string FormattedCamera { 
            get {
                if (CameraMaker != null && CameraModel != null)
                    return CameraMaker + " " + CameraModel;
                else if (CameraModel != null)
                    return CameraModel;
                else if (CameraMaker != null)
                    return CameraMaker;
                return null;
            } 
        }
        public string FormattedResolution
        {
            get
            {
                return PictureWidth.ToString() + " x " + PictureHeight.ToString(); 
            }
        }
    }
}
