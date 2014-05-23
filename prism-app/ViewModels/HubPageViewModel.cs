using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
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
        public HubPageViewModel() 
        {
            Groups = new ObservableCollection<PictureCategoryViewModel>();
            PictureSelectedCommand = new DelegateCommand<PictureViewModel>(GotoPictureDetail);
        }

        private void GotoPictureDetail(PictureViewModel picture)
        {
            NavigationService.Navigate("PictureDetail", picture);
        }

        public DelegateCommand<PictureViewModel> PictureSelectedCommand { get; private set; }

        public ObservableCollection<PictureCategoryViewModel> Groups { get; private set; }

        private void GotoCategroyDetail(PictureCategoryViewModel category)
        {
            NavigationService.Navigate("CategoryDetail", category);
        }

        private bool CanGotoCategoryDetail(PictureCategoryViewModel category)
        {
            return category.Items.Count > 0;
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                string serverUrl = ResourceDictionary["BaseThumbnailUrl"] as string;
                var result = await ExecuteAsync(new SearchTopPicturesByCategoriesRequest { MaxAge = 180 * Constants.MillisPerDay, GroupSize = 10, Categories = Constants.AllCategories });
                var groupSelectionCommand = new DelegateCommand<PictureCategoryViewModel>(GotoCategroyDetail, CanGotoCategoryDetail);
                foreach (var category in result.Keys.Where((key) => result[key].Count > 0).OrderBy((key) => key))
                {
                    Groups.Add(new PictureCategoryViewModel(category, new Uri(serverUrl, UriKind.Absolute), result[category], groupSelectionCommand));
                }
            }
            catch (Exception)
            {
                Groups.Clear();
            }
        }
    }
}
