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
    public sealed class PictureDetailPageViewModel : LoadableViewModel
    {
        private PictureViewModel info;

        [Dependency]
        public PictureCommentUserControlViewModel Comments { get; set; }

        [Dependency]
        public IApplicationStateService ApplicationStateService { get; set; }

        [InjectionMethod]
        public void Init()
        {
            ApplicationStateService.PropertyChanged += ApplicationStateService_PropertyChanged;
            ApplicationStateService_PropertyChanged(this, new PropertyChangedEventArgs(ExtractMemberName(() => ApplicationStateService.CurrentUser)));
        }

        private async void ApplicationStateService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ExtractMemberName(() => ApplicationStateService.CurrentUser) == e.PropertyName)
            {
                if (Info != null)
                {
                    try
                    {
                        await LoadData(Info.PictureUri);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                await LoadData(new Uri(navigationParameter as string, UriKind.Relative));
            }
            catch (Exception)
            {
                Info = null;
                NavigationService.GoBack();
            }
            
        }

        private async Task LoadData(Uri pictureUri)
        {
            var picture = await ExecuteAsync(new ViewPictureDetailRequest() { PictureUri = pictureUri });
            string serverUrl = ResourceDictionary["BasePreviewUrl"] as string;
            Info = new PictureViewModel(new Uri(serverUrl, UriKind.Absolute), picture);
            Comments.LoadComments(Info);
        }

        public PictureViewModel Info
        {
            get { return info; }
            private set { SetProperty(ref info, value); }
        }
    }
}
