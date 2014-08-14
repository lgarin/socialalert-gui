using Socialalert.Services;
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
}
