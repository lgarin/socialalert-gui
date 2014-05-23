using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Test.Services
{
    [TestClass]
    public class JsonRpcClientFixture
    {
        private JsonRpcClient client;
        private Uri serverUrl;

        [TestInitialize]
        public void init()
        {
            client = new JsonRpcClient();
            serverUrl = new Uri("http://jcla3ndtozbxyghx.myfritz.net:18789/socialalert-app/rest/");
        }

        [TestMethod]
        public void TestBasicSearch()
        {
            var result = client.InvokeAsync<QueryResult<PictureInfo>>(serverUrl, new SearchPicturesRequest { MaxAge = 4000000000, PageSize = 5, Keywords = "Tag" }).Result;
            Assert.AreEqual(3, result.PageCount);
            Assert.AreEqual(0, result.PageNumber);
            Assert.AreEqual(5, result.Content.Length);
        }

        [TestMethod]
        public void TestInvalidSearch()
        {
            try
            {
                var result = client.InvokeAsync<QueryResult<PictureInfo>>(serverUrl, new SearchPicturesRequest { MaxAge = 400000000 }).Result;
                Assert.Fail();
            } catch (AggregateException e) {
                Assert.IsInstanceOfType(e.InnerException, typeof(JsonRpcException));
            }
        }
    }
}
