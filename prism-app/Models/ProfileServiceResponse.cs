using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Models
{
    public class PublicProfileInfo
    {
        public Guid ProfileId;
        public Uri Image;
        public String Nickname;
        public String Biography;
        public Boolean Online;
    }

    public class ProfileStatisticInfo : PublicProfileInfo
    {
        public int PictureCount;
        public int CommentCount;
        public int HitCount;
        public int LikeCount;
        public int DislikeCount;
        public int FollowerCount;
    }
}
