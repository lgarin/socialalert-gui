using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.StoreApps;
using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public sealed class ProfileStatisticViewModel : SimpleViewModel
    {
        public ProfileStatisticViewModel(string profileUriPattern, ProfileStatisticInfo info)
        {
            ProfileId = info.ProfileId;
            ProfileUriPattern = profileUriPattern;
            UpdateWith(info);
        }

        public String ProfileUriPattern { get; private set; }

        private void UpdateWith(ProfileStatisticInfo info)
        {
            ImageUrl = new Uri(string.Format(ProfileUriPattern, ProfileId), UriKind.Absolute);
            Nickname = info.Nickname;
            Creator = info.Nickname;
            Biography = info.Biography;
            Online = info.Online;
            PictureCount = info.PictureCount;
            CommentCount = info.CommentCount;
            HitCount = info.HitCount;
            LikeCount = info.LikeCount;
            DislikeCount = info.DislikeCount;
            FollowerCount = info.FollowerCount;
        }

        public Guid ProfileId { get { return Get<Guid>(); } set { Set(value); } }
        public Uri ImageUrl { get { return Get<Uri>(); } set { Set(value); } }
        public String Nickname { get { return Get<String>(); } set { Set(value); } }
        public String Creator { get { return Get<String>(); } set { Set(value); } }
        public String Biography { get { return Get<String>(); } set { Set(value); } }
        public Boolean Online { get { return Get<Boolean>(); } set { Set(value); } }

        public int PictureCount { get { return Get<int>(); } set { Set(value); } }
        public int CommentCount { get { return Get<int>(); } set { Set(value); } }
        public int HitCount { get { return Get<int>(); } set { Set(value); } }
        public int LikeCount { get { return Get<int>(); } set { Set(value); } }
        public int DislikeCount { get { return Get<int>(); } set { Set(value); } }
        public int FollowerCount { get { return Get<int>(); } set { Set(value); } }

        public bool IsFollowed { get { return Get<bool>(); } set { Set(value); } }
        public DelegateCommand ToggleFollowCommand { get { return Get<DelegateCommand>(); } set { Set(value); } }
    }
}
