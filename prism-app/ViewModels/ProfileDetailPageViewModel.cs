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
        ProfileStatisticInfo Info;

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                await LoadData(new Guid(navigationParameter as string));
            }
            catch (Exception)
            {
                Info = null;
                NavigationService.GoBack();
            }
            
        }

        private async Task LoadData(Guid profileId)
        {
            Info = await ExecuteAsync(new GetUserProfileRequest(profileId));
        }
    }
}
