using Microsoft.Practices.Unity;
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

        [Dependency]
        public PictureCommentUserControlViewModel Comments { get; set; }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                string serverUrl = ResourceDictionary["BaseImageUrl"] as string;
                var picture = await ExecuteAsync(new ViewPictureDetailRequest(navigationParameter as string));
                Info = new PictureViewModel(new Uri(serverUrl, UriKind.Absolute), picture);
                Comments.LoadComments(picture.PictureUri);
            }
            catch (Exception)
            {
                Info = null;
                NavigationService.GoBack();
            }
            
        }

        public PictureViewModel Info
        {
            get { return info; }
            private set { SetProperty(ref info, value); }
        }
    }
}
