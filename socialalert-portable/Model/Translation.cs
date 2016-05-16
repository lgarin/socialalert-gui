using Xamarin.Forms;

namespace Bravson.Socialalert.Portable.Model
{
    static class Translation
    {
        public static string GetErrorMessage(this ErrorCode errorCode, ResourceDictionary resources)
        {
            switch (errorCode)
            {
                case ErrorCode.BadBredentials: return "Bad credential";
                case ErrorCode.LockedAccount: return "Locked account";
                case ErrorCode.NetworkFailure: return "Cannot connect to server";
                default: return "Unkwnown error";
            }
        }

        public static string Translate(this string key, ResourceDictionary resources)
        {
            return key;
        }
    }
}
