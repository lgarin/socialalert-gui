using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable.Model
{
    public class PictureGridItem : SimpleModel
    {
        public PictureGridItem(IEnumerable<MediaInfo> mediaInfoGroup, Uri baseThumbnailUri)
        {
            ImageSources = mediaInfoGroup.Select(x => x == null ? null : ImageSource.FromUri(new Uri(baseThumbnailUri, x.MediaUri))).ToArray();
        }

        public ImageSource[] ImageSources
        {
            get { return Get<ImageSource[]>(); }
            set { Set(value);  }
        }
    }
}
