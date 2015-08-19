using Microsoft.VisualStudio.TestTools.UnitTesting;
using Socialalert.Services;
using Windows.Devices.Geolocation;

namespace Socialalert.Test.Services
{
    [TestClass]
    public class GeoLocationServiceFixture
    {
        GeoLocationService service;

        [TestInitialize]
        public void init()
        {
            service = new GeoLocationService();
        }

        //[TestMethod]
        public void TestGetCurrentLocation()
        {
            var location = service.GetCurrentLocation(PositionAccuracy.Default);
            location.Wait();
            Assert.IsNotNull(location.Result);
        }
    }
}
