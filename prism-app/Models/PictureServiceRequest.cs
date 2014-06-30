using Newtonsoft.Json;
using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Socialalert.Services
{

    public class ViewPictureDetailRequest : JsonRpcRequest<PictureInfo> 
    {

        public Uri PictureUri;

        public ViewPictureDetailRequest() : base("pictureFacade", "viewPictureDetail") { }

        public ViewPictureDetailRequest(string pictureUri) : this()
        {
            PictureUri = new Uri(pictureUri, UriKind.Relative);
        }
    }

    public class SearchTopPicturesByCategoriesRequest : JsonRpcRequest<IDictionary<String, IList<PictureInfo>>> 
    {
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int GroupSize;
        public string[] Categories;
        public double? Longitude;
        public double? Latitude;

        public SearchTopPicturesByCategoriesRequest() : base("pictureFacade", "searchTopPicturesByCategories") { }
    }

    public class SearchPicturesRequest : JsonRpcRequest<QueryResult<PictureInfo>>
    {
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int PageSize;
        public int PageNumber;
        public double? Longitude;
        public double? Latitude;

        public SearchPicturesRequest() : base("pictureFacade", "searchPictures") { }
    }

    public class SearchPicturesInCategoryRequest : JsonRpcRequest<QueryResult<PictureInfo>>
    {
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int PageSize;
        public int PageNumber;
        public double? Longitude;
        public double? Latitude;
        public string Category;

        public SearchPicturesInCategoryRequest() : base("pictureFacade", "searchPicturesInCategory") { }
    }

    public class FindKeywordSuggestionsRequest : JsonRpcRequest<IEnumerable<String>>
    {
        public string Partial;

        public FindKeywordSuggestionsRequest() : base("pictureFacade", "findKeywordSuggestions") { }

        public FindKeywordSuggestionsRequest(string partial) : this()
        {
            Partial = partial;
        }
    }

    public class ListCommentsRequest : JsonRpcRequest<QueryResult<CommentInfo>>
    {
        public Uri PictureUri;
        public int PageSize;
        public int PageNumber;

        public ListCommentsRequest() : base("pictureFacade", "listComments") { }
    }

    public class MapPictureMatchCountRequest : JsonRpcRequest<IList<GeoStatistic>>
    {
        public long MaxAge;
        public string Keywords;
        public double Longitude;
        public double Latitude;
        public double Radius;
        public Guid[] ProfileId;

        public MapPictureMatchCountRequest() : base("pictureFacade", "mapPictureMatchCount") {}
    }
}
