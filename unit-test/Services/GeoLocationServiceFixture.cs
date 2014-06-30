using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
