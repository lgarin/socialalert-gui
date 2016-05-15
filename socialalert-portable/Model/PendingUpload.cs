using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
