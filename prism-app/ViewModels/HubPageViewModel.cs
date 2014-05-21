using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public class HubPageViewModel : LoadableViewModel
    {
        private INavigationService _navigationService;

        public HubPageViewModel(INavigationService navigationService, JsonRpcClient rpcClient, IAlertMessageService alertMessageService, IResourceLoader resourceLoader) : base(rpcClient, alertMessageService, resourceLoader)
        {
            _navigationService = navigationService;
            Groups = new ObservableCollection<PictureCategoryViewModel>();
            PictureSelectedCommand = new DelegateCommand<PictureViewModel>((picture) => _navigationService.Navigate("PictureDetail", picture));
        }

        public DelegateCommand<PictureViewModel> PictureSelectedCommand { get; private set; }

        public ObservableCollection<PictureCategoryViewModel> Groups { get; private set; }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                string serverUrl = Application.Current.Resources["BaseThumbnailUrl"] as string;
                var result = await ExecuteAsync(new SearchTopPicturesByCategoriesRequest { MaxAge = 180 * Constants.MillisPerDay, GroupSize = 10, Categories = Constants.AllCategories });
                foreach (var category in result.Keys.Where((key) => result[key].Count > 0).OrderBy((key) => key))
                {
                    Groups.Add(new PictureCategoryViewModel(category, new Uri(serverUrl, UriKind.Absolute), result[category]));
                }
            }
            catch (Exception)
            {
                Groups.Clear();
            }
        }
    }
}
