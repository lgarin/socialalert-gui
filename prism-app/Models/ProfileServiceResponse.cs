using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Models
{
    [JsonObject]
    public class PublicProfileInfo
    {
        public Guid ProfileId;
        public Uri Image;
        public String Nickname;
        public String Biography;
        public Boolean Online;
    }

    [JsonObject]
    public class ProfileStatisticInfo : PublicProfileInfo
    {
        public int PictureCount;
        public int CommentCount;
        public int HitCount;
        public int LikeCount;
        public int DislikeCount;
        public int FollowerCount;
    }

    public enum AbuseReason
    {
        VIOLENCE,
        SEX,
        BAD_LANGUAGE,
        DRUGS,
        DISCRIMINATION
    }

    public enum AbuseStatus {
	    NEW,
	    PROCESSING,
	    CLOSED
    }

    [JsonObject]
    public class AbuseInfo
    {
        public Uri mediaUri;

        public Guid profileId;

        public AbuseReason reason;

        public AbuseStatus status;

        public String country;

        public DateTime timestamp;

        public Guid commentId;

        public String message;

        public String creator;

        public Boolean online;
    }
}
