using Bravson.Socialalert.Portable.Model;

using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public partial class App : Application
    {
        public static AppState State { get; } = new AppState();

        public new static App Current
        {
            get { return Application.Current as App; }
        }

        public static JsonRpcClient Connection { get; private set; }

        public App()
        {
            InitializeComponent();
            Connection = new JsonRpcClient(Resources["BaseServerUrl"] as string);
            if (Properties.ContainsKey("State"))
            {
                State.Populate(Properties["State"] as string);
            }
            //MainPage = new LoginPage();
            MainPage = new NavigationPage(new PictureGridPage()); 
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            Properties["State"] = State.Serialize();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
