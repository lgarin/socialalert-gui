using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Models
{
    static class Constants
    {
        public const long MillisPerDay = 1000L * 60L * 60L * 24L;

        public static readonly string[] AllCategories = {"ART",
	        "PLACES",
	        "NEWS",
	        "PARTY",
	        "AWESOME",
	        "BUZZ",
	        "NATURE",
	        "SELFIES"};
    }
}
