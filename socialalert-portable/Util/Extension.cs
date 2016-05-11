using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bravson.Socialalert.Portable.Util
{
    public static class Extension
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int batchSize) where T : class
        {
            return items.Select((item, index) => new { item, index }).GroupBy(pair => pair.index / batchSize, pair => pair.item);
        }
    }
}
