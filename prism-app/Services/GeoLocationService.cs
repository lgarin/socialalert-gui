using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Socialalert.Services
{
    public interface IGeoLocationService
    {
        Task<Location> GetCurrentLocation(PositionAccuracy accuracy = PositionAccuracy.Default);

        LocationRect ComputeLocationBounds(IEnumerable<Location> locations);

        double ComputeRadiusInKm(LocationRect box);

        bool AreClose(LocationRect rect1, LocationRect rect2, double percentage);
    }

    public sealed class GeoLocationService : IGeoLocationService
    {
        private const double DEGREES_TO_RADIANS =  Math.PI / 180;
        private const double EARTH_MEAN_RADIUS_KM = 6371.0087714;

        private readonly Geolocator locator = new Geolocator();

        public async Task<Location> GetCurrentLocation(PositionAccuracy accuracy)
        {
            locator.DesiredAccuracy = accuracy;
            var location = await locator.GetGeopositionAsync();
            return new Location(location.Coordinate.Point.Position.Latitude, location.Coordinate.Point.Position.Longitude);
        }

        public LocationRect ComputeLocationBounds(IEnumerable<Location> locations)
        {
            LocationCollection collection = new LocationCollection();
            foreach (var loc in locations) {
                collection.Add(loc);
            }
            return new LocationRect(collection);
        }

        public double ComputeRadiusInKm(LocationRect box)
        {
            return Math.Max(box.Height, box.Width) * DEGREES_TO_RADIANS * EARTH_MEAN_RADIUS_KM / 2.0;
        }

        public bool AreClose(LocationRect rect1, LocationRect rect2, double percentage)
        {
            if (Math.Abs(rect1.Width - rect2.Width) > 180.0 * percentage)
            {
                return false;
            }
            if (Math.Abs(rect1.Height - rect2.Height) > 90.0 * percentage)
            {
                return false;
            }
            var averageWidth = (rect1.Width + rect2.Width) / 2;
            if (Math.Abs(rect1.East - rect2.East) > averageWidth * percentage)
            {
                return false;
            }
            if (Math.Abs(rect1.West - rect2.West) > averageWidth * percentage)
            {
                return false;
            }
            var averageHeigth = (rect1.Height + rect2.Height) / 2;
            if (Math.Abs(rect1.North - rect2.North) > averageHeigth * percentage)
            {
                return false;
            }
            if (Math.Abs(rect1.South - rect2.South) > averageHeigth * percentage)
            {
                return false;
            }
            return true;
        }
    }
}
