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
    }

    public sealed class GeoLocationService : IGeoLocationService
    {
        private readonly Geolocator locator = new Geolocator();

        public async Task<Location> GetCurrentLocation(PositionAccuracy accuracy)
        {
            locator.DesiredAccuracy = accuracy;
            var location = await locator.GetGeopositionAsync();
            return new Location(location.Coordinate.Latitude, location.Coordinate.Longitude);
        }

        public LocationRect ComputeLocationBounds(IEnumerable<Location> locations)
        {
            LocationCollection collection = new LocationCollection();
            foreach (var loc in locations) {
                collection.Add(loc);
            }
            return new LocationRect(collection);
        }
    }
}
