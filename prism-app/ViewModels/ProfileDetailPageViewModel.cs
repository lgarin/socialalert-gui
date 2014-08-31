using Microsoft.Practices.Prism.StoreApps;
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
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public sealed class ProfileDetailPageViewModel : LoadableViewModel
    {
        private ProfileStatisticViewModel info;

        [Dependency]
        public ProfileFeedUserControlViewModel Activites { get; set; }

        [Dependency]
        public IApplicationStateService ApplicationStateService { get; set; }

        [InjectionMethod]
        public void Init()
        {
            ApplicationStateService.PropertyChanged += ApplicationStateService_PropertyChanged;
        }

        private async void ApplicationStateService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Info != null && ExtractMemberName(() => ApplicationStateService.CurrentUser) == e.PropertyName)
            {

                try
                {
                    await RefreshState();
                }
                catch (Exception)
                {
                    Info.IsFollowed = false;
                    Info.ToggleFollowCommand = new DelegateCommand(ToggleFollow, CanToggleFollow);
                }
            }
        }

        private async void ToggleFollow()
        {
            if (Info.IsFollowed)
            {
                await ExecuteAsync(new UnfollowProfileRequest(Info.ProfileId));
                Info.IsFollowed = false;
            }
            else
            {
                Info.IsFollowed = await ExecuteAsync(new FollowProfileRequest(Info.ProfileId));
            }
        }

        private bool CanToggleFollow()
        {
            return Info != null && ApplicationStateService.CurrentUser != null && Info.ProfileId != ApplicationStateService.CurrentUser.ProfileId;
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                await LoadData((Guid) navigationParameter);
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

        private async Task LoadData(Guid profileId)
        {
            var profile = await ExecuteAsync(new GetUserProfileRequest(profileId));
            Info = new ProfileStatisticViewModel(ProfileUriPattern, profile);
            await RefreshState();
        }

        private async Task RefreshState()
        {
            if (ApplicationStateService.CurrentUser != null)
            {
                Info.IsFollowed = await ExecuteAsync(new IsFollowingProfileRequest(Info.ProfileId));
            }
            else
            {
                Info.IsFollowed = false;
            }
            Info.ToggleFollowCommand = new DelegateCommand(ToggleFollow, CanToggleFollow);
            Activites.Load(Info);
        }

        public ProfileStatisticViewModel Info
        {
            get { return info; }
            private set { SetProperty(ref info, value); }
        }
    }
}
