using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bravson.Socialalert.Portable.Util
{
    public static class Extension
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int batchSize) where T : class
        {
            return items.Select((item, index) => new { item, index }).GroupBy(pair => pair.index / batchSize, pair => pair.item);
        }

        public static object Get(this IDictionary<string, object> dictionary, string key)
        {
            object value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }

        public static long GetEpochMillis(this DateTime dateTime)
        {
            DateTime dateTimeUtc = dateTime;
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dateTimeUtc = dateTime.ToUniversalTime();
            }

            if (dateTimeUtc <= UnixEpoch)
            {
                return 0;
            }

            return (dateTimeUtc.Ticks - UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond;
        }

        public static DateTime FromEpochMillis(this long millis)
        {
            return UnixEpoch.AddTicks(millis * TimeSpan.TicksPerMillisecond);
        }
    }
}
