using System;

namespace Bravson.Socialalert.Portable.Data
{
    public class GetUserProfileRequest : JsonRpcRequest<ProfileStatisticInfo>
    {
        public Guid ProfileId;

        public GetUserProfileRequest() : base("profileFacade", "getUserProfile") { }

        public GetUserProfileRequest(Guid profileId) : this()
        {
            ProfileId = profileId;
        }
    }

    public class IsFollowingProfileRequest : JsonRpcRequest<DateTime>
    {
        public Guid ProfileId;

        public IsFollowingProfileRequest() : base("profileFacade", "isFollowingSince") { }

        public IsFollowingProfileRequest(Guid profileId) : this()
        {
            ProfileId = profileId;
        }
    }

    public class FollowProfileRequest : JsonRpcRequest<Boolean>
    {
        public Guid ProfileId;

        public FollowProfileRequest() : base("profileFacade", "follow") { }

        public FollowProfileRequest(Guid profileId)
            : this()
        {
            ProfileId = profileId;
        }
    }

    public class UnfollowProfileRequest : JsonRpcRequest<Boolean>
    {
        public Guid ProfileId;

        public UnfollowProfileRequest() : base("profileFacade", "unfollow") { }

        public UnfollowProfileRequest(Guid profileId)
            : this()
        {
            ProfileId = profileId;
        }
    }

    public class GetProfileActivity : JsonRpcRequest<QueryResult<ActivityInfo>>
    {
        public Guid ProfileId;
        public Int32 PageNumber;
        public Int32 PageSize;

        public GetProfileActivity() : base("profileFacade", "getProfileActivity") { }
    }

    public class ReportAbusiveComment : JsonRpcRequest<QueryResult<AbuseInfo>>
    {
        public Guid CommentId;
        public String Country;
        public String Reason;

        public ReportAbusiveComment() : base("profileFacade", "reportAbusiveComment") { }
    }

    public class ReportAbusiveMedia : JsonRpcRequest<QueryResult<AbuseInfo>>
    {
        public Uri MediaId;
        public String Country;
        public String Reason;

        public ReportAbusiveMedia() : base("profileFacade", "reportAbusiveMedia") { }
    }
}
