﻿using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Models
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

    public class IsFollowingProfileRequest : JsonRpcRequest<Boolean>
    {
        public Guid ProfileId;

        public IsFollowingProfileRequest() : base("profileFacade", "isFollowing") { }

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
}
