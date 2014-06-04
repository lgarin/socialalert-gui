using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Socialalert.Converters
{
    public sealed class ViewChangeEndedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = value as ViewChangeEndedEventArgs;
            var map = parameter as Map;
            if (args != null & map != null)
            {
                return map.Bounds;
            }
                

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
