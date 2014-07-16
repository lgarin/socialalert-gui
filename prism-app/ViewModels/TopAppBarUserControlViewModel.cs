using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
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

        public TopAppBarUserControlViewModel()
        {
            DumpDataCommand = new DelegateCommand(DumpData);
            LoginCommand = new DelegateCommand(Login);
        }

        private async void Login() {

            bool retry = false;
            do
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
            } while (retry);
        }

        private async Task<bool> DoLogin(PasswordCredential crendential)
        {
            try
            {
                UserInfo userInfo = await ExecuteAsync(new LoginRequest(crendential.UserName, crendential.Password));
                // TODO store the userInfo
                return false;
            }
            catch (JsonRpcException)
            {
                return true;
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
