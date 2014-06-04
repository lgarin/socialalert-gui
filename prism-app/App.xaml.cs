using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Socialalert.Services;
using Socialalert.ViewModels;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Socialalert
{
    sealed partial class App : MvvmAppBase, IDisposable
    {
        private readonly IUnityContainer container = new UnityContainer();

        public App()
        {
            this.InitializeComponent();
        }

        protected override Task OnLaunchApplication(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("Hub", null);
			return Task.FromResult<object>(null);
        }

        protected override void OnInitialize(IActivatedEventArgs args)
        {
            container.RegisterInstance(NavigationService);
            container.RegisterInstance(Resources);

            container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            container.RegisterInstance<IAlertMessageService>(new AlertMessageService());
            container.RegisterInstance<IJsonRpcClient>(new JsonRpcClient());
            container.RegisterInstance<IGeoLocationService>(new GeoLocationService());
        }

        protected override object Resolve(Type type)
        {
            return container.Resolve(type);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}
