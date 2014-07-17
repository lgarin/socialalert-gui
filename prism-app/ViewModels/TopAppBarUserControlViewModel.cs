using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
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

namespace Socialalert.ViewModels
{
    public class TopAppBarUserControlViewModel : LoadableViewModel
    {
        [Dependency]
        public IUnityContainer Container {get; set;}
        
        public DelegateCommand DumpDataCommand { get; private set; }

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
            
        }

        [InjectionMethod]
        public void Init()
        {
            ApplicationStateService.PropertyChanged += ApplicationStateService_PropertyChanged;
            ApplicationStateService_PropertyChanged(null, null);
        }

        private void ApplicationStateService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(() => this.CurrentUsername);
            OnPropertyChanged(() => this.CanLogin);
            OnPropertyChanged(() => this.CanLogout);
            LoginCommand.RaiseCanExecuteChanged();
            LogoutCommand.RaiseCanExecuteChanged();
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
            catch (JsonRpcException)
            {
            }
        }

        private void DumpData()
        {
            var viewModel = Container.Resolve<PictureCommentUserControlViewModel>();
            string json = viewModel.SerializeToJson();
            Debug.WriteLine(viewModel.GetType().Name);
            Debug.WriteLine(json);
            Debug.WriteLine("----------------------------------");
        }

    }
}
