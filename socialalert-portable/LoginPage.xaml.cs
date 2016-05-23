using Bravson.Socialalert.Portable.Data;
using Bravson.Socialalert.Portable.Model;
using System;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public partial class LoginPage : ModelPage
    {
        public LoginPage()
        {
            InitializeComponent();
            SizeChanged += OnPageSizeChanged;

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
                App.UploadService.Run(); 
                App.Current.MainPage = new PictureGridPage();
            }
            catch (Exception e)
            {
                DisplayError("Login failed".Translate(Resources), e);
                usernameEntry.Focus();
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        void OnPageSizeChanged(object sender, EventArgs args)
        {
            BatchBegin();

            // Portrait mode. 
            if (Width < Height)
            {
                paddingBox.IsVisible = true;
                mainGrid.VerticalOptions = LayoutOptions.Start;
                mainGrid.RowDefinitions[1].Height = GridLength.Auto;
                mainGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Absolute);
                Grid.SetRow(controlPanelStack, 1);
                Grid.SetColumn(controlPanelStack, 0);
            }
            // Landscape mode. 
            else
            {
                paddingBox.IsVisible = false;
                mainGrid.VerticalOptions = LayoutOptions.Center;
                mainGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Absolute);
                mainGrid.ColumnDefinitions[1].Width = GridLength.Auto;
                Grid.SetRow(controlPanelStack, 0);
                Grid.SetColumn(controlPanelStack, 1);
            }

            BatchCommit();
        }
    }
}
