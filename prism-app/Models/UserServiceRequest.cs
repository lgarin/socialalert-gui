using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Models
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
