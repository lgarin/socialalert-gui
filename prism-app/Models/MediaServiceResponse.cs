using Newtonsoft.Json;
using System;

namespace Socialalert.Models
{
    [JsonObject]
    public class QueryResult<T>
    {
        public T[] Content;
        public int PageNumber;
        public int PageCount;
    }

    public enum MediaType
    {
        PICTURE,
        VIDEO
    }

    public enum UserApprovalModifier
    {
        LIKE,
        DISLIKE
    }

    [JsonObject]
    public class MediaInfo
    {
        public Uri MediaUri;
        public MediaType MediaType;
        public string Title;
        public string Description;
        public Guid ProfileId;
        public DateTime Creation;
        public DateTime LastUpdate;
        public DateTime? Timestamp;
        public int Width;
        public int Height;
        public double? Longitude;
        public double? Latitude;
        public string Locality;
        public string Country;
        public string CameraMaker;
        public string CameraModel;
        public int HitCount;
        public int LikeCount;
        public int DislikeCount;
        public int CommentCount;
        public string[] Categories;
        public string[] Tags;
        public UserApprovalModifier? UserApprovalModifier;
        public string Creator;
        public bool Online;
    }

    [JsonObject]
    public class CommentInfo
    {
	    public Guid CommentId;
        public Uri MediaUri;
        public Guid ProfileId;
        public DateTime Creation;
        public string Comment;
        public string Creator;
        public bool Online;
    }

    [JsonObject]
    public class GeoArea
    {
        public double Latitude;
        public double Longitude;
        public double Radius;
    }


    [JsonObject]
    public class GeoStatistic : GeoArea
    {
        public long Count;
    }

    public enum ActivityType {
	    NEW_PICTURE,
	    REPOST_PICTURE,
	    NEW_COMMENT,
	    REPOST_COMMENT,
	    LIKE_MEDIA,
	    UNLIKE_MEDIA
    }

    [JsonObject]
    public class ActivityInfo
    {
        public Uri MediaUri;

        public Guid ProfileId;

        public ActivityType ActivityType;

        public DateTime Timestamp;

        public Guid? CommentId;

        public String Message;

        public String Creator;

        public Boolean Online;
    }
}