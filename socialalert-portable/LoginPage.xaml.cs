using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public partial class LoginPage : ModelPage
    {
        public LoginPage()
        {
            InitializeComponent();
            Username = App.State.DefaultUsername;
            if (string.IsNullOrEmpty(Username))
            {
                usernameEntry.Focus();
            }
            else
            {
                passwordEntry.Focus();
            }
        }

        public string Username
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public string Password
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public Command Login
        {
            get { return Command(DoLogin, CanLogin); }
        }

        private async void DoLogin()
        {
            try
            {
                App.State.UserInfo = await ExecuteAsync(new LoginRequest(Username, Password));
                App.State.DefaultUsername = Username;
            }
            catch (Exception e)
            {
                DisplayError("Login failed", e);
                usernameEntry.Focus();
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }
    }
}
