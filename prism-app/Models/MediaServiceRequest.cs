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

    public class ViewMediaDetailRequest : JsonRpcRequest<MediaInfo> 
    {

        public Uri MediaUri;

        public ViewMediaDetailRequest() : base("mediaFacade", "viewMediaDetail") { }

        public ViewMediaDetailRequest(string mediaUri) : this()
        {
            MediaUri = new Uri(mediaUri, UriKind.Relative);
        }
    }

    public class SearchTopMediaByCategoriesRequest : JsonRpcRequest<IDictionary<String, IList<MediaInfo>>> 
    {
        public MediaType MediaType;
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int GroupSize;
        public string[] Categories;
        public double? Longitude;
        public double? Latitude;

        public SearchTopMediaByCategoriesRequest() : base("mediaFacade", "searchTopMediaByCategories") { }
    }

    public class SearchMediaRequest : JsonRpcRequest<QueryResult<MediaInfo>>
    {
        public MediaType MediaType;
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int PageSize;
        public int PageNumber;
        public double? Longitude;
        public double? Latitude;

        public SearchMediaRequest() : base("mediaFacade", "searchMedia") { }
    }

    public class SearchMediaInCategoryRequest : JsonRpcRequest<QueryResult<MediaInfo>>
    {
        public MediaType MediaType;
        public long MaxAge;
        public string Keywords;
        public double? MaxDistance;
        public int PageSize;
        public int PageNumber;
        public double? Longitude;
        public double? Latitude;
        public string Category;

        public SearchMediaInCategoryRequest() : base("mediaFacade", "searchMediaInCategory") { }
    }

    public class FindKeywordSuggestionsRequest : JsonRpcRequest<IEnumerable<String>>
    {
        public MediaType MediaType;
        public string Partial;

        public FindKeywordSuggestionsRequest() : base("mediaFacade", "findKeywordSuggestions") { }

        public FindKeywordSuggestionsRequest(string partial) : this()
        {
            Partial = partial;
        }
    }

    public class ListCommentsRequest : JsonRpcRequest<QueryResult<CommentInfo>>
    {
        public Uri MediaUri;
        public int PageSize;
        public int PageNumber;

        public ListCommentsRequest() : base("mediaFacade", "listComments") { }
    }

    public class MapMediaMatchCountRequest : JsonRpcRequest<IList<GeoStatistic>>
    {
        public MediaType MediaType;
        public long MaxAge;
        public string Keywords;
        public double Longitude;
        public double Latitude;
        public double Radius;
        public Guid[] ProfileId;

        public MapMediaMatchCountRequest() : base("mediaFacade", "mapMediaMatchCount") {}
    }

    public class SetMediaApprovalRequest : JsonRpcRequest<MediaInfo>
    {
        public Uri MediaUri;
        public UserApprovalModifier? Modifier;

        public SetMediaApprovalRequest() : base("mediaFacade", "setMediaApproval") { }
    }

    public class AddCommentRequest : JsonRpcRequest<CommentInfo>
    {
        public Uri MediaUri;
        public string Comment;

        public AddCommentRequest() : base("mediaFacade", "addComment") { }
    }

    public class RepostMediaRequest : JsonRpcRequest<ActivityInfo>
    {
        public Uri MediaUri;

        public RepostMediaRequest() : base("mediaFacade", "repostMedia") { }
    }

    public class RepostCommentRequest : JsonRpcRequest<ActivityInfo>
    {
        public Guid CommentId;

        public RepostCommentRequest() : base("mediaFacade", "repostComment") { }
    }

    public class ListMediaByProfile : JsonRpcRequest<QueryResult<MediaInfo>>
    {
        public MediaType MediaType;
        public Guid ProfileId;
        public int PageSize;
        public int PageNumber;

        public ListMediaByProfile() : base("mediaFacade", "listMediaByProfile") { }
    }
}
