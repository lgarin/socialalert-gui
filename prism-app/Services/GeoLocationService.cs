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
        Task<Geopoint> GetCurrentLocation(PositionAccuracy accuracy);
    }

    public sealed class GeoLocationService : IGeoLocationService
    {
        private readonly Geolocator locator = new Geolocator();

        public async Task<Geopoint> GetCurrentLocation(PositionAccuracy accuracy)
        {
            locator.DesiredAccuracy = accuracy;
            var location = await locator.GetGeopositionAsync();
            return location.Coordinate.Point;
        }
    }
}
