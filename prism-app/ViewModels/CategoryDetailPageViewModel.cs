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

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                var category = navigationParameter as string;
                var pictures = new IncrementalLoadingCollection<PictureViewModel>(LoadMorePictures);
                Category = new PictureCategoryViewModel(category, pictures, new DelegateCommand<PictureCategoryViewModel>(GotoCategoryDetail));
            }
            catch (Exception)
            {
                Category = null;
                NavigationService.GoBack();
            }
        }

        private async Task<IEnumerable<PictureViewModel>> LoadMorePictures(int pageIndex, int pageSize)
        {
            try
            {
                var items = await ExecuteAsync(new SearchPicturesInCategoryRequest { MaxAge = 180 * Constants.MillisPerDay, PageNumber = pageIndex, PageSize = pageSize, Category = Category.Id });
                var basePictureUri = new Uri(ResourceDictionary["BaseThumbnailUrl"] as string, UriKind.Absolute);
                var result = new List<PictureViewModel>(items.Content.Count());
                foreach (var item in items.Content)
                {
                    result.Add(new PictureViewModel(basePictureUri, item));
                }
                return result;
            }
            catch (Exception)
            {
                return Enumerable.Empty<PictureViewModel>();
            }
        }

        public PictureCategoryViewModel Category
        {
            get { return _category; }
            private set { SetProperty(ref _category, value); }
        }
    }
}
