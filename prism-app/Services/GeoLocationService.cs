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
            return Math.Max(box.Height, box.Width) * DEGREES_TO_RADIANS * EARTH_MEAN_RADIUS_KM;
        }
    }
}
