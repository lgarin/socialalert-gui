namespace Bravson.Socialalert.Portable
{
    static class Constants
    {
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

    static class ErrorCode
    {
        public const int BAD_CREDENTIALS = -1;
        public const int CREDENTIAL_EXPIRED = -2;
        public const int LOCKED_ACCOUNT = -3;
        public const int ACCESS_DENIED = -4;
        public const int INVALID_INPUT = -5;
        public const int NO_CREDENTIALS_FOUND = -6;
        public const int USER_NOT_FOUND = -7;
        public const int DUPLICATE_KEY = -8;
        public const int NON_UNIQUE_INPUT = -9;
        public const int UNSAFE_PASSWORD = -10;
        public const int SYSTEM_ERROR = -500;
        public const int UNSPECIFIED = -999;
    }
}
