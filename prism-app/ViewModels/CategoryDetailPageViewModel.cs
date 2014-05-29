using Microsoft.Practices.Prism.StoreApps;
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
    public sealed class CategoryDetailPageViewModel : LoadableViewModel
    {

        private PictureCategoryViewModel _category;

        public CategoryDetailPageViewModel() 
        {
            PictureSelectedCommand = new DelegateCommand<PictureViewModel>(GotoPictureDetail);
        }

        public DelegateCommand<PictureViewModel> PictureSelectedCommand { get; private set; }

        private void GotoPictureDetail(PictureViewModel picture)
        {
            NavigationService.Navigate("PictureDetail", picture.PictureUri.OriginalString);
        }

        private void GotoCategoryDetail(PictureCategoryViewModel category)
        {
            NavigationService.Navigate("CategoryDetail", category.Id);
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                var category = navigationParameter as string;
                var serverUrl = ResourceDictionary["BaseImageUrl"] as string;
                var result = await ExecuteAsync(new SearchPicturesInCategoryRequest { MaxAge = 180 * Constants.MillisPerDay, PageSize = 20, Category = category });
                Category = new PictureCategoryViewModel(category, new Uri(serverUrl, UriKind.Absolute), result.Content, new DelegateCommand<PictureCategoryViewModel>(GotoCategoryDetail));
            }
            catch (Exception)
            {
                Category = null;
                NavigationService.GoBack();
            }
        }

        public PictureCategoryViewModel Category
        {
            get { return _category; }
            private set { SetProperty(ref _category, value); }
        }
    }
}
