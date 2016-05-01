using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    class LoginModel : LoadableModel
    {
        public LoginModel(Page page) : base(page)
        {
            
        }

        public string Username { 
            get { return Get<string>(); }
            set { Set(value); }
        }

        public string Password
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public ICommand Login
        {
            get { return Command(DoLogin, CanLogin); }
        }

        private async void DoLogin()
        {
            try
            {
                UserInfo user = await ExecuteAsync(new LoginRequest(Username, Password));
                Debug.WriteLine(user.Nickname);
            }
            catch (JsonRpcException e)
            {
                    
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }
        
    }
}
