using Microsoft.Practices.Prism.PubSubEvents;
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
            container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAlertMessageService, AlertMessageService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IJsonRpcClient, JsonRpcClient>(new ContainerControlledLifetimeManager());
            container.RegisterType<IGeoLocationService, GeoLocationService>(new ContainerControlledLifetimeManager());
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
