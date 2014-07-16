using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;

namespace Socialalert.Services
{
    public interface ILoginCredentialService
    {
        PasswordCredential FindPreviousCredential();
        Task<PasswordCredential> GetCrendential(bool retry = false);
    }

    public class LoginCredentialService : ILoginCredentialService
    {
        private readonly string resourceName;
        private readonly IResourceLoader resourceLoader;

        public LoginCredentialService(IResourceLoader resourceLoader)
        {
            this.resourceLoader = resourceLoader;
            this.resourceName = resourceLoader.GetString("CredentialResourceKey");
        }

        public PasswordCredential FindPreviousCredential()
        {
            var passwordVault = new PasswordVault();
            try
            {
                PasswordCredential previousCredential = passwordVault.FindAllByResource(resourceName).First();
                previousCredential.RetrievePassword();
                return previousCredential;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void StoreCredential(PasswordCredential credential)
        {
            var passwordVault = new PasswordVault();
            try
            {
                foreach (var previousCredential in passwordVault.FindAllByResource(resourceName))
                {
                    passwordVault.Remove(previousCredential);
                }
            }
            catch (Exception)
            {
                // ignore
            }
            passwordVault.Add(credential);
        }

        private async Task<CredentialPickerResults> AskCredential(bool retry)
        {
            var options = new CredentialPickerOptions()
            {
                AuthenticationProtocol = AuthenticationProtocol.Basic,
                CredentialSaveOption = CredentialSaveOption.Selected,
                CallerSavesCredential = true,
                Caption = retry ? resourceLoader.GetString("CredentialPickerRetryCaption") : resourceLoader.GetString("CredentialPickerCaption"),
                Message = retry ? resourceLoader.GetString("CredentialPickerRetryMessage") : resourceLoader.GetString("CredentialPickerMessage"),
                TargetName = "."
            };
            
            return await CredentialPicker.PickAsync(options);
        }

        public async Task<PasswordCredential> GetCrendential(bool retry)
        {
            PasswordCredential previousCredential = retry ? null : FindPreviousCredential();
            if (previousCredential != null)
            {
                return previousCredential;
            }

            var results = await AskCredential(retry);
            if (results.ErrorCode != 0) {
                return null;
            }

            PasswordCredential newCredential = new PasswordCredential(resourceName, results.CredentialUserName, results.CredentialPassword);
            if (results.CredentialSaveOption == CredentialSaveOption.Selected)
            {
                StoreCredential(newCredential);
            }
            return newCredential;
        }
    }
}
