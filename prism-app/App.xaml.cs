using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using Socialalert.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Security.Credentials;
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
            return OpenAsync();
        }

        protected override void OnInitialize(IActivatedEventArgs args)
        {

            Suspending += AppSuspending;
            Resuming += AppResuming;

            container.RegisterInstance(NavigationService);
            container.RegisterInstance(Resources);

            container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            container.RegisterInstance<IAlertMessageService>(new AlertMessageService());
            container.RegisterInstance<IJsonRpcClient>(new JsonRpcClient(Resources["BaseServerUrl"] as string));
            container.RegisterInstance<IGeoLocationService>(new GeoLocationService());
            container.RegisterInstance<ILoginCredentialService>(new LoginCredentialService(container.Resolve<IResourceLoader>()));
            container.RegisterInstance<IApplicationStateService>(new ApplicationStateService(new SessionStateService()));
        }

        private async void AppResuming(object sender, object e)
        {
            await OpenAsync();
        }

        private async Task OpenAsync()
        {
            await container.Resolve<IApplicationStateService>().RestoreAsync();

            var credentialService = container.Resolve<ILoginCredentialService>();
            PasswordCredential crendential = credentialService.FindPreviousCredential();
            if (crendential != null)
            {
                UserInfo userInfo = await container.Resolve<IJsonRpcClient>().InvokeAsync(new LoginRequest(crendential.UserName, crendential.Password));
                container.Resolve<IApplicationStateService>().CurrentUser = userInfo;
            }
        }

        private async void CloseAsync(SuspendingDeferral deferral)
        {
            if (container.Resolve<IApplicationStateService>().CurrentUser != null)
            {
                await container.Resolve<IJsonRpcClient>().InvokeAsync(new LogoutRequest());
                container.Resolve<IApplicationStateService>().CurrentUser = null;
            }
            
            await container.Resolve<IApplicationStateService>().SaveAsync();
            deferral.Complete();
        }

        private void AppSuspending(object sender, SuspendingEventArgs e)
        {
            CloseAsync(e.SuspendingOperation.GetDeferral());
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
