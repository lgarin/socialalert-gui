using Newtonsoft.Json;
using Socialalert.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Socialalert.UI.Data
{
    [JsonObject]
    public abstract class PictureServiceRequest<T>
    {
        private static readonly JsonRpcClient rpcClient;

        private readonly string methodName;

        static PictureServiceRequest()
        {
            string serverUrl = Application.Current.Resources["BaseServerUrl"] as string;
            rpcClient = new JsonRpcClient(new Uri(new Uri(serverUrl), "pictureFacade"));
        }

        protected PictureServiceRequest(string methodName)
        {
            this.methodName = methodName;
        }

        public async Task<T> ExecuteAsync()
        {
            return await rpcClient.InvokeAsync<T>(methodName, this);
        }
    }

    public class ViewPictureDetailRequest : PictureServiceRequest<PictureInfo> 
    {

        public Uri PictureUri;

        public ViewPictureDetailRequest() : base("viewPictureDetail") { }

        public ViewPictureDetailRequest(string pictureUri) : this()
        {
            PictureUri = new Uri(pictureUri, UriKind.Relative);
        }
    }

    public class SearchTopPicturesByCategoriesRequest : PictureServiceRequest<IDictionary<String, IList<PictureInfo>>> 
    {
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int GroupSize;
        public string[] Categories;
        public double? Longitude;
        public double? Latitude;

        public SearchTopPicturesByCategoriesRequest() : base("searchTopPicturesByCategories") { }
    }

    public class SearchPicturesRequest : PictureServiceRequest<QueryResult<PictureInfo>>
    {
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int PageSize;
        public int PageNumber;
        public double? Longitude;
        public double? Latitude;

        public SearchPicturesRequest() : base("searchPictures") { }
    }

    public class SearchPicturesInCategoryRequest : PictureServiceRequest<QueryResult<PictureInfo>>
    {
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int PageSize;
        public int PageNumber;
        public double? Longitude;
        public double? Latitude;
        public string Category;

        public SearchPicturesInCategoryRequest() : base("searchPicturesInCategory") { }
    }
}
