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
        public PictureGridItem(MediaInfo info, Uri baseThumbnailUri)
        {
            ImageSource = ImageSource.FromUri(new Uri(baseThumbnailUri, info.MediaUri));
        }

        public ImageSource ImageSource
        {
            get { return Get<ImageSource>(); }
            set { Set(value);  }
        }
    }
}
