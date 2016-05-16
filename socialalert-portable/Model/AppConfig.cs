using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable.Model
{
    public sealed class AppConfig
    {
        public AppConfig(ResourceDictionary resources)
        {
            ServerUrl = new Uri(resources["ServerUrl"] as string, UriKind.Absolute);
            BaseThumbnailUri = new Uri(resources["BaseThumbnailUrl"] as string, UriKind.Absolute);
        }

        public Uri ServerUrl { get; private set; }

        public Uri BaseThumbnailUri { get; private set; } 
    }
}
