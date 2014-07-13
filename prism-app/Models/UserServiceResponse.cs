using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Models
{
    public enum UserRole
    {
        USER, ADMINISTRATOR, GUEST, THIRD_PARTY, SYSTEM, BACK_OFFICE, ANONYMOUS
    }

    public enum UserState
    {
        UNVERIFIED,
        ACTIVE,
        PASSWORD_EXPIRED,
        LOCKED
    }

    [JsonObject]
    public class UserInfo
    {
        public String Email;

        public String Nickname;

        public UserRole[] Roles;

        public UserState State;

        public DateTime Creation;

        public DateTime LastUpdate;

        public DateTime? LastLoginSuccess;

        public DateTime? LastLoginFailure;

        public Int32 LoginFailureCount;

        public Guid? ProfileId;
    }
}
