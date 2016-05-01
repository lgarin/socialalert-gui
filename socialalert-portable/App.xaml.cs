using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public partial class App : Application
    {
        public AppState State { get; } = new AppState();

        public new static App Current
        {
            get { return Application.Current as App; }
        }

        private JsonRpcClient client;

        public App()
        {
            InitializeComponent();
            client = new JsonRpcClient(Resources["BaseServerUrl"] as string);
            if (Properties.ContainsKey("State"))
            {
                State.Populate(Properties["State"] as string);
            }
            MainPage = new LoginPage();
        }

        public async Task<T> RequestServerAsync<T>(JsonRpcRequest<T> requestObject)
        {
            return await client.InvokeAsync(requestObject);
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
