using System;

namespace Bravson.Socialalert.Portable.Data
{
    public class LoginRequest : JsonRpcRequest<UserInfo>
    {

        public String Email;

        public String Password;

        public LoginRequest() : base("userFacade", "login") { }

        public LoginRequest(String email, String password)
            : this()
        {
            Email = email;
            Password = password;
        }
    }

    public class LogoutRequest : JsonRpcRequest<Object>
    {
        public LogoutRequest() : base("userFacade", "logout") { }
    }
}
