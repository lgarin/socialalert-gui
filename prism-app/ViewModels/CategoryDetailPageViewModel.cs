using Bing.Maps;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private bool mapLocked;
        private string keywords;
        private LocationRect mapBounds;
        private LocationRect searchBounds;
        private PictureCategoryViewModel category;

        [Dependency]
        public SearchPictureUserControlViewModel PictureSearch { get; set; }

        [Dependency]
        public IGeoLocationService LocationService { get; set; }

        public CategoryDetailPageViewModel() 
        {
            PictureSelectedCommand = new DelegateCommand<PictureViewModel>(CenterMapOnPicture);
            MapViewChangedCommand = new DelegateCommand<LocationRect>(StartMapSearch);
            LockMapCommand = new DelegateCommand<object>(LockMap);
            UnlockMapCommand = new DelegateCommand<object>(UnlockMap);
        }

        private void CenterMapOnPicture(PictureViewModel picture)
        {
            if (SearchBounds == null && MapBounds == null || picture == null)
            {
                return;
            }

            if (picture.HasGeoLocation)
            {
                var bounds = SearchBounds ?? MapBounds;
                MapBounds = new LocationRect(picture.GeoLocation, bounds.Width, bounds.Height);
            }
        }

        private void LockMap(object arg)
        {
            MapLocked = true;
            SearchBounds = MapBounds;
            TriggerNewSearch();
        }

        private void UnlockMap(object arg)
        {
            MapLocked = false;
            SearchBounds = null;
            TriggerNewSearch();
        }

        private void StartMapSearch(LocationRect box)
        {
            if (SearchBounds == null || Category == null)
            {
                return;
            }

            if (!LocationService.AreClose(SearchBounds, box, 0.20))
            {
                SearchBounds = box;
                TriggerNewSearch();
            }
        }

        public DelegateCommand<object> UnlockMapCommand { get; private set; }

        public DelegateCommand<object> LockMapCommand { get; private set; }

        public DelegateCommand<LocationRect> MapViewChangedCommand { get; private set; }

        public DelegateCommand<PictureViewModel> PictureSelectedCommand { get; private set; }

        private void GotoPictureDetail(PictureViewModel picture)
        {
            NavigationService.Navigate("PictureDetail", picture.PictureUri.OriginalString);
        }

        private void GotoCategoryDetail(PictureCategoryViewModel category)
        {
            NavigationService.Navigate("CategoryDetail", category.Id);
        }

        private void StartKeywordSearch(string keywords = null)
        {
            if (!object.Equals(Keywords, keywords)) { 
                Keywords = keywords;
                //SearchBounds = null;
                TriggerNewSearch();
            }
        }

        private async void TriggerNewSearch()
        {
            if (Category != null)
            {
                Category.SetItems(await LoadMorePictures(Keywords, SearchBounds));
                ComputeMapBounds();
            }
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            try
            {
                MapLocked = false;
                var category = navigationParameter as string;
                Category = new PictureCategoryViewModel(category, new DelegateCommand<PictureCategoryViewModel>(GotoCategoryDetail));
                PictureSearch.SearchAction = new Action<string>(StartKeywordSearch);
                TriggerNewSearch();
            }
            catch (Exception)
            {
                MapBounds = null;
                Keywords = null;
                SearchBounds = null;
                Category = null;
                NavigationService.GoBack();
            }
        }

        private void ComputeMapBounds()
        {
            if (SearchBounds == null)
            {
                LocationRect tempBound = LocationService.ComputeLocationBounds(Category.GeoLocatedItems.Select((i) => i.GeoLocation));
                MapBounds = new LocationRect(tempBound.Center, tempBound.Width * 1.2, tempBound.Height * 1.2);
            }
        }

        private async Task<IEnumerable<PictureViewModel>> LoadMorePictures(string keywords, LocationRect searchBounds)
        {
            try
            {
                double? maxDistance = null;
                double? longitude = null;
                double? latitude = null;
                if (searchBounds != null)
                {
                    maxDistance = LocationService.ComputeRadiusInKm(searchBounds);
                    longitude = searchBounds.Center.Longitude;
                    latitude = searchBounds.Center.Latitude;
                }
                var items = await ExecuteAsync(new SearchPicturesInCategoryRequest { Keywords = keywords, MaxDistance = maxDistance, Latitude = latitude, Longitude = longitude, MaxAge = 360 * Constants.MillisPerDay, PageNumber = 0, PageSize = Constants.ItemsPerPage, Category = Category.Id });
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

        public LocationRect SearchBounds
        {
            get { return searchBounds; }
            private set { 
                SetProperty(ref searchBounds, value);
            }
        }

        public string Keywords
        {
            get { return keywords;  }
            private set { SetProperty(ref keywords, value);  }
        }

        public bool MapLocked
        {
            get { return mapLocked; }
            private set { SetProperty(ref mapLocked, value); }
        }
    }
}

