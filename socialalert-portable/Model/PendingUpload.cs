using Bravson.Socialalert.Portable.Data;
using SQLite;
using System;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable.Model
{
    public enum UploadState
    {
        Pending,
        Uploading,
        WaitingInput,
        Claiming,
        Completed,
        Error
    }

    public static class UploadStateExtension
    {
        public static string GetDescription(this UploadState state, ResourceDictionary resources)
        {
            switch (state)
            {
                case UploadState.Pending: return "Upload pending";
                case UploadState.Uploading: return "Upload in progress";
                case UploadState.WaitingInput: return "Waiting user input";
                case UploadState.Claiming: return "Claiming media";
                case UploadState.Error: return "Upload failed";
                case UploadState.Completed: return "Upload completed";
                default: return "Processing upload";
            }
        }
    }

    [Table("Upload")]
    public class PendingUpload
    {
        public PendingUpload()
        {

        }

        public PendingUpload(MediaType mediaType, string filePath)
        {
            Timestamp = DateTime.Now;
            MediaType = mediaType;
            FilePath = filePath;
            State = UploadState.Pending;
        }

        [PrimaryKey, NotNull, AutoIncrement, Column("_id")]
        public int? Id { get; set; }

        [NotNull]
        public MediaType MediaType { get; set; }

        [Unique]
        public string FilePath { get; set; }

        [NotNull]
        public DateTime Timestamp { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string Uri { get; set; }

        public MediaCategory? Category { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }

        public string Country { get; set; }

        public string Locality { get; set; }

        public string Address { get; set; }

        [Ignore]
        public UploadState State { get; set; } 

        public UploadState DetermineState()
        {
            if (Uri == null)
            {
                return UploadState.Pending;
            }
            else if (Title == null)
            {
                return UploadState.WaitingInput;
            }
            else
            {
                return UploadState.Claiming;
            }
        }

        [Ignore]
        public bool Ongoing
        {
            get
            {
                return State != UploadState.Completed;
            }
        }

        [Ignore]
        public Color Color
        {
            get
            {
                switch (State)
                {
                    case UploadState.Error: return Color.Red;
                    case UploadState.WaitingInput: return Color.Lime;
                    case UploadState.Completed: return Color.Green;
                    case UploadState.Uploading: return Color.Blue;
                    case UploadState.Claiming: return Color.Blue;
                    default: return Color.Black;
                }
            }
        }

        [Ignore]
        public string ContentType
        {
            get
            {
                switch (MediaType)
                {
                    case MediaType.PICTURE: return Constants.JpegMimeType;
                    case MediaType.VIDEO: return Constants.Mp4MimeType;
                    default: return null;
                }
            }
        }

        [Ignore]
        public bool CanClaim
        {
            get { return Uri != null && Title != null; }
        }

        [Ignore]
        public MediaCategory[] CategoryArray
        {
            get { return Category.HasValue ? new MediaCategory[] { Category.Value } : new MediaCategory[0]; }
        }

        [Ignore]
        public string[] TagArray
        {
            get { return string.IsNullOrEmpty(Tags) ? new string[0] : Tags.Split(' '); }
        }

        [Ignore]
        public Uri RelativeUri
        {
            get { return Uri != null ? new Uri(Uri, UriKind.Relative) : null;  }
        }
    }
}
