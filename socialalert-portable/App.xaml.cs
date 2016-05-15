using Bravson.Socialalert.Portable.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public partial class App : Application
    {
        public new static App Current
        {
            get { return Application.Current as App; }
        }

        public static AppState State { get; private set; }

        public static JsonRpcClient ServerConnection { get; private set; }

        public static SqliteClient DatabaseConnection { get; private set; }

        public static NotificationClient Notification { get; private set; }

        public App()
        {
            InitializeComponent();

            Notification = new NotificationClient();
        }

        private Task InitServerConnection(string serverUrl)
        {
            return Task.Run(() => ServerConnection = new JsonRpcClient(serverUrl));
        }

        private Task InitDatabaseConnection(string databaseName)
        {
            return Task.Run(() => DatabaseConnection = new SqliteClient(databaseName));
        }

        private Task InitAppState(IDictionary<string, object> persistedState)
        {
            if (persistedState.ContainsKey("State"))
            {
                return Task.Run(() => State = new AppState(persistedState["State"] as string));
            }
            return Task.FromResult(0);
        }

        protected async override void OnStart()
        {
            // Handle when your app starts
            await Task.WhenAll(InitDatabaseConnection("sqlite.db3"),
                               InitServerConnection(Resources["BaseServerUrl"] as string),
                               InitAppState(Properties));

            MainPage = new PictureGridPage();
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
