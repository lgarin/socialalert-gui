using Bing.Maps;
using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Socialalert.ViewModels
{
    public sealed class GeoStatisticViewModel : SimpleViewModel
    {
        public GeoStatisticViewModel(GeoStatistic item)
        {
            Latitude = item.Latitude;
            Longitude = item.Longitude;
            Radius = item.Radius;
            Count = item.Count;
        }

        public double Latitude { get { return Get<double>(); } set { Set(value); } }
        public double Longitude { get { return Get<double>(); } set { Set(value); } }
        public double Radius { get { return Get<double>(); } set { Set(value); } }
        public long Count { get { return Get<long>(); } set { Set(value); } }

        public Location GeoLocation { get { return new Location(Latitude, Longitude); } }
        public Point ElipseAnchor { get { return new Point(ElipseWidth / 2, ElipseHeight / 2); } }
        public int ElipseHeight { get { return (int) Math.Ceiling(Radius / 2.0);  } }
        public int ElipseWidth { get { return (int)Math.Ceiling(Radius / 2.0); } }
    }
}
