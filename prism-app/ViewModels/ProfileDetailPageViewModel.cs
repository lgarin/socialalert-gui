using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public sealed class ProfileDetailPageViewModel : LoadableViewModel
    {
        private ProfileStatisticViewModel info;

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                await LoadData(navigationParameter as Guid?);
            }
            catch (Exception)
            {
                Info = null;
                NavigationService.GoBack();
            }
            
        }

        private string ProfileUriPattern
        {
            get
            {
                var baseProfileUri = ResourceDictionary["BaseThumbnailUrl"] as string;
                var profileUriPattern = ResourceDictionary["ProfilePictureUriPattern"] as string;
                return baseProfileUri + profileUriPattern;
            }
        }

        private async Task LoadData(Guid? profileId)
        {
            var profile = await ExecuteAsync(new GetUserProfileRequest(profileId.Value));
            Info = new ProfileStatisticViewModel(ProfileUriPattern, profile);
        }

        public ProfileStatisticViewModel Info
        {
            get { return info; }
            private set { SetProperty(ref info, value); }
        }
    }
}
