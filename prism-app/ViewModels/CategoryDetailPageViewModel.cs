using Bing.Maps;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public sealed class CategoryDetailPageViewModel : LoadableViewModel
    {

        private PictureCategoryViewModel category;
        private LocationRect mapBounds;
        private IGeoLocationService geoLoationService;
        private string keywords;

        public CategoryDetailPageViewModel(IEventAggregator eventAggregator, IGeoLocationService geoLoationService) 
        {
            this.geoLoationService = geoLoationService;
            PictureSelectedCommand = new DelegateCommand<PictureViewModel>(GotoPictureDetail);
            eventAggregator.GetEvent<SearchPictureUserControlEvent>().Subscribe(ReloadPictures);
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

        private void ReloadPictures(string keywords = null)
        {
            this.keywords = keywords;
            if (Category != null)
            {
                Category.Items.Clear();
            }
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                var category = navigationParameter as string;
                var pictures = new IncrementalLoadingCollection<PictureViewModel>(LoadMorePictures);
                Category = new PictureCategoryViewModel(category, pictures, new DelegateCommand<PictureCategoryViewModel>(GotoCategoryDetail));
                Category.GeoLocatedItems.CollectionChanged += (s, e) => ComputeMapBounds();
            }
            catch (Exception)
            {
                MapBounds = null;
                Category = null;
                NavigationService.GoBack();
            }
        }

        void ComputeMapBounds()
        {
            LocationRect tempBound = geoLoationService.ComputeLocationBounds(Category.GeoLocatedItems.Select((i) => i.GeoLocation));
            MapBounds = new LocationRect(tempBound.Center, tempBound.Width + 2, tempBound.Height + 2);
        }

        private async Task<IEnumerable<PictureViewModel>> LoadMorePictures(int pageIndex, int pageSize)
        {
            try
            {
                var items = await ExecuteAsync(new SearchPicturesInCategoryRequest { Keywords = keywords, MaxAge = 180 * Constants.MillisPerDay, PageNumber = pageIndex, PageSize = pageSize, Category = Category.Id });
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
            get { return category; }
            private set { SetProperty(ref category, value); }
        }

        public LocationRect MapBounds
        {
            get { return mapBounds; }
            private set { SetProperty(ref mapBounds, value); }
        }
    }
}
