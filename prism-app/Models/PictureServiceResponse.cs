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

    [JsonObject]
    public class PictureInfo
    {
        public Uri PictureUri;
        public string Title;
        public string Description;
        public Guid ProfileId;
        public DateTime Creation;
        public DateTime LastUpdate;
        public DateTime? PictureTimestamp;
        public int PictureWidth;
        public int PictureHeight;
        public double? PictureLongitude;
        public double? PictureLatitude;
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
        public string UserApprovalModifier;
        public string Creator;
        public bool Online;
    }
}