using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
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

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            EventAggregator.GetEvent<DumpDataUserControlEvent>().Subscribe(WriteJson);
            try
            {
                string serverUrl = ResourceDictionary["BaseImageUrl"] as string;
                var picture = await ExecuteAsync(new ViewPictureDetailRequest(navigationParameter as string));
                Info = new PictureViewModel(new Uri(serverUrl, UriKind.Absolute), picture);
            }
            catch (Exception)
            {
                Info = null;
                NavigationService.GoBack();
            }
            
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            EventAggregator.GetEvent<DumpDataUserControlEvent>().Unsubscribe(WriteJson);
            base.OnNavigatedFrom(viewModelState, suspending);
        }

        public PictureViewModel Info
        {
            get { return info; }
            private set { SetProperty(ref info, value); }
        }
    }
}
