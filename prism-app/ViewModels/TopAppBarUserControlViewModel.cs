using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using Socialalert.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Socialalert.ViewModels
{
    public class TopAppBarUserControlViewModel : LoadableViewModel
    {
        [Dependency]
        public IUnityContainer Container {get; set;}

        public DelegateCommand DumpDataCommand { get; private set; }
        public DelegateCommand GoHomeCommand { get; private set; }
        public DelegateCommand LoginCommand { get; private set; }
        public DelegateCommand LogoutCommand { get; private set; }

        [Dependency]
        public ILoginCredentialService LoginCredentialService { get; set; }

        [Dependency]
        public IApplicationStateService ApplicationStateService { get; set; }

        public TopAppBarUserControlViewModel()
        {
            DumpDataCommand = new DelegateCommand(DumpData);
            LoginCommand = new DelegateCommand(Login, () => CanLogin);
            LogoutCommand = new DelegateCommand(Logout, () => CanLogout);
            GoHomeCommand = new DelegateCommand(GoHome, () => CanGoHome);
        }

        [InjectionMethod]
        public void Init()
        {
            ApplicationStateService.PropertyChanged += ApplicationStateService_PropertyChanged;
            ApplicationStateService_PropertyChanged(this, new PropertyChangedEventArgs(ExtractMemberName(() => ApplicationStateService.CurrentUser)));
        }

        private void ApplicationStateService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ExtractMemberName(() => ApplicationStateService.CurrentUser) == e.PropertyName) {
                OnPropertyChanged(() => CurrentUsername);
                OnPropertyChanged(() => CanLogin);
                OnPropertyChanged(() => CanLogout);
                LoginCommand.RaiseCanExecuteChanged();
                LogoutCommand.RaiseCanExecuteChanged();
            }
        }

        public void GoHome()
        {
            NavigationService.Navigate(ResourceDictionary["HomePage"] as string, null);
        }

        public bool CanGoHome
        {
            get
            {
                var frame = Window.Current.Content as Frame;
                if (frame != null)
                {
                    return frame.SourcePageType.FullName != ResourceDictionary["HomePage"] as string;
                }
                return true;
            }
        }

        public string CurrentUsername
        {
            get
            {
                return ApplicationStateService.CurrentUser != null ? ApplicationStateService.CurrentUser.Nickname : null;
            }
        }

        public bool CanLogin
        {
            get
            {
                return ApplicationStateService.CurrentUser == null;
            }
        }

        private async void Login() {

            bool retry = true;
            while (retry)
            {            
                PasswordCredential crendential = await LoginCredentialService.GetCrendential(retry);
                if (crendential != null)
                {
                    retry = await DoLogin(crendential);
                }
                else
                {
                    retry = false;
                }
            }
        }

        private async Task<bool> DoLogin(PasswordCredential crendential)
        {
            try
            {
                UserInfo userInfo = await ExecuteAsync(new LoginRequest(crendential.UserName, crendential.Password));
                ApplicationStateService.CurrentUser = userInfo;
                return false;
            }
            catch (JsonRpcException)
            {
                return true;
            }
        }

        public bool CanLogout
        {
            get
            {
                return ApplicationStateService.CurrentUser != null;
            }
        }

        private async void Logout()
        {
            try
            {
                await ExecuteAsync(new LogoutRequest());
                ApplicationStateService.CurrentUser = null;
            }
            catch (JsonRpcException e)
            {
                if (e.HResult == -4)
                {
                    ApplicationStateService.CurrentUser = null;
                }
            }
        }

        private void DumpData()
        {
            
            foreach (ContainerRegistration registration in Container.Registrations) {
                LoadableViewModel viewModel = Container.Resolve(registration.RegisteredType) as LoadableViewModel;
                if (viewModel != null)
                {
                    string json = viewModel.SerializeToJson();
                    Debug.WriteLine(viewModel.GetType().Name);
                    Debug.WriteLine(json);
                    Debug.WriteLine("----------------------------------");
                }
            }
            
        }

    }
}
