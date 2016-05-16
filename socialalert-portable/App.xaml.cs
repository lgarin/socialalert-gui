using Bravson.Socialalert.Portable.Model;
using Bravson.Socialalert.Portable.Util;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public partial class App : Application
    {
        private const string AppStatePropertyKey = "State";

        public new static App Current
        {
            get { return Application.Current as App; }
        }

        public static AppState State { get; private set; }

        public static AppConfig Config { get; private set; }

        public static JsonRpcClient ServerConnection { get; private set; }

        public static DatabaseClient DatabaseConnection { get; private set; }

        public static NotificationClient Notification { get; private set; }

        public static UploadService UploadService { get; private set; }

        public App()
        {
            InitializeComponent();
            Config = new AppConfig(Resources);
            Notification = new NotificationClient(Resources);
            UploadService = new UploadService();
        }

        private Task InitServerConnection(Uri serverUrl)
        {
            return Task.Run(() => ServerConnection = new JsonRpcClient(serverUrl));
        }

        private Task InitDatabaseConnection(string databaseName)
        {
            return Task.Run(() => DatabaseConnection = new DatabaseClient(databaseName));
        }

        private Task InitAppState(string persistedState)
        {
            return Task.Run(() => State = new AppState(persistedState));
        }

        protected async override void OnStart()
        {
            // Handle when your app starts
            await Task.WhenAll(InitDatabaseConnection("sqlite.db3"),
                               InitServerConnection(Config.ServerUrl),
                               InitAppState(Properties.Get(AppStatePropertyKey) as string));

            UploadService.Run();
            MainPage = new PictureGridPage();
        }

        protected override void OnSleep()
        {
            Properties[AppStatePropertyKey] = State.Serialize();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
