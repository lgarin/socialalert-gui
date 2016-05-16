namespace Bravson.Socialalert.Portable.Model
{
    static class Constants
    {
        public const string JpegMimeType = "image/jpeg";
        public const string Mp4MimeType = "video/mp4";

        public const long MillisPerDay = 1000L * 60L * 60L * 24L;

        public const int ItemsPerPage = 20;

        public static readonly string[] AllCategories = {"ART",
	        "PLACES",
	        "NEWS",
	        "PARTY",
	        "AWESOME",
	        "BUZZ",
	        "NATURE",
	        "SELFIES"};
    }

    public enum ErrorCode
    {
        BadBredentials = -1,
        CredentialExpired = -2,
        LockedAccount = -3,
        AccessDenied = -4,
        InvalidInput = -5,
        NoCredentialsFound = -6,
        UserNotFound = -7,
        DuplicateKey = -8,
        NonUniqueInput = -9,
        UnsafePassword = -10,
        SystemError = -500,
        Unspecified = -999,
        NetworkFailure = -9999
    }
}
