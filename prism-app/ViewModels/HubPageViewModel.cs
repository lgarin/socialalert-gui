using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public sealed class HubPageViewModel : LoadableViewModel
    {
        public HubPageViewModel() 
        {
            Groups = new ObservableCollection<PictureCategoryViewModel>();
            PictureSelectedCommand = new DelegateCommand<PictureViewModel>(GotoPictureDetail);
        }

        private void GotoPictureDetail(PictureViewModel picture)
        {
            NavigationService.Navigate("PictureDetail", picture.PictureUri.OriginalString);
        }

        public DelegateCommand<PictureViewModel> PictureSelectedCommand { get; private set; }

        public ObservableCollection<PictureCategoryViewModel> Groups { get; private set; }

        private void GotoCategroyDetail(PictureCategoryViewModel category)
        {
            NavigationService.Navigate("CategoryDetail", category.Id);
        }

        private bool CanGotoCategoryDetail(PictureCategoryViewModel category)
        {
            return category != null && category.Items.Count > 0;
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                var basePictureUri = new Uri(ResourceDictionary["BaseThumbnailUrl"] as string, UriKind.Absolute);
                var result = await ExecuteAsync(new SearchTopPicturesByCategoriesRequest { MaxAge = 180 * Constants.MillisPerDay, GroupSize = 10, Categories = Constants.AllCategories });
                var groupSelectionCommand = new DelegateCommand<PictureCategoryViewModel>(GotoCategroyDetail, CanGotoCategoryDetail);
                foreach (var category in result.Keys.Where((key) => result[key].Count > 0).OrderBy((key) => key))
                {
                    var items = new ObservableCollection<PictureViewModel>();
                    foreach (PictureInfo picture in result[category]) {
                        items.Add(new PictureViewModel(basePictureUri, picture));
                    }
                    Groups.Add(new PictureCategoryViewModel(category, items, groupSelectionCommand));
                }
            }
            catch (Exception)
            {
                Groups.Clear();
            }
        }
    }
}
