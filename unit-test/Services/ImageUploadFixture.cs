using Microsoft.VisualStudio.TestTools.UnitTesting;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Socialalert.Test.Services
{
    [TestClass]
    public class ImageUploadFixture
    {
        ImageUploadService service;

        [TestInitialize]
        public void init()
        {
            service = new ImageUploadService("http://jcla3ndtozbxyghx.myfritz.net:18789/socialalert-app/upload");
        }

        private async Task<Uri> UploadTempFileAsync()
        {
            var images = await Package.Current.InstalledLocation.GetFolderAsync("Images");
            var file = await images.GetFileAsync("TestUploadPicture.jpg");
            return await service.PostPictureAsync(await file.OpenReadAsync());
        }

        [TestMethod]
        public void UploadTempFile()
        {
            var result = UploadTempFileAsync();
            result.Wait();
            Assert.AreEqual(new Uri("test", UriKind.Relative), result.Result);
        }


        [TestCleanup]
        public void Dispose()
        {
            if (service != null)
            {
                service.Dispose();
            }
        }
    }
}
