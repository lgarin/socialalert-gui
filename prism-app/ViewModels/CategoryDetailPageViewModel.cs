using Bing.Maps;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
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
        private LocationRect mapBounds;
        private IGeoLocationService geoLoationService;
        private PictureCategoryViewModel category;

        public CategoryDetailPageViewModel(IGeoLocationService geoLoationService) 
        {
            this.geoLoationService = geoLoationService;
            PictureSelectedCommand = new DelegateCommand<PictureViewModel>(GotoPictureDetail);
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            EventAggregator.GetEvent<SearchPictureUserControlEvent>().Unsubscribe(ReloadPictures);
            EventAggregator.GetEvent<DumpDataUserControlEvent>().Unsubscribe(WriteJson);
            base.OnNavigatedFrom(viewModelState, suspending);
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
            if (Category != null)
            {
                Category.Items = new IncrementalLoadingCollection<PictureViewModel>((i, s) => LoadMorePictures(keywords, i, s));
                Category.GeoLocatedItems.CollectionChanged += (s, e) => ComputeMapBounds();
            }
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
            EventAggregator.GetEvent<SearchPictureUserControlEvent>().Subscribe(ReloadPictures);
            EventAggregator.GetEvent<DumpDataUserControlEvent>().Subscribe(WriteJson);
            try
            {
                var category = navigationParameter as string;
                Category = new PictureCategoryViewModel(category, new DelegateCommand<PictureCategoryViewModel>(GotoCategoryDetail));
                ReloadPictures();
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

        private async Task<IEnumerable<PictureViewModel>> LoadMorePictures(string keywords, int pageIndex, int pageSize)
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
