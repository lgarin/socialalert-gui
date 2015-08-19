using Bing.Maps;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public class MapStatisticPageViewModel : LoadableViewModel
    {
        private string keywords;
        private LocationRect mapBounds;
        private LocationRect searchBounds;

        public MapStatisticPageViewModel()
        {
            Items = new ObservableCollection<GeoStatisticViewModel>();
            MapViewChangedCommand = new DelegateCommand<LocationRect>(StartMapSearch);
        }

        [Dependency]
        public SearchPictureUserControlViewModel KeywordSearch { get; set; }

        [Dependency]
        public IGeoLocationService LocationService { get; set; }

        public DelegateCommand<LocationRect> MapViewChangedCommand { get; private set; }

        private async void TriggerNewSearch()
        {
            var radius = LocationService.ComputeRadiusInKm(searchBounds);
            var longitude = searchBounds.Center.Longitude;
            var latitude = searchBounds.Center.Latitude;
            var result = await ExecuteAsync(new MapPictureMatchCountRequest { Keywords = Keywords, MaxAge = Constants.MillisPerDay * 1000, Radius = radius, Latitude = latitude, Longitude = longitude });
            Items.Clear();
            foreach (var item in result)
            {
                Items.Add(new GeoStatisticViewModel(item));
            }
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            KeywordSearch.SearchAction = new Action<string>(StartKeywordSearch);
            //try
            //{
            //    SearchBounds = new LocationRect(new Location(), 1000, 1000);
            //    TriggerNewSearch();
            //}
            //catch (Exception)
            //{
            //    MapBounds = null;
            //    Keywords = null;
            //    Items.Clear();
            //    NavigationService.GoBack();
            //}
        }

        private void StartMapSearch(LocationRect box)
        {
            if (SearchBounds == null || !LocationService.AreClose(SearchBounds, box, 0.20))
            {
                SearchBounds = box;
                TriggerNewSearch();
            }
        }

        private void StartKeywordSearch(string keywords = null)
        {
            if (!object.Equals(Keywords, keywords))
            {
                Keywords = keywords;
                TriggerNewSearch();
            }
        }

        public string Keywords
        {
            get { return keywords; }
            private set { SetProperty(ref keywords, value); }
        }

        public LocationRect MapBounds
        {
            get { return mapBounds; }
            private set { SetProperty(ref mapBounds, value); }
        }

        public LocationRect SearchBounds
        {
            get { return searchBounds; }
            private set
            {
                SetProperty(ref searchBounds, value);
            }
        }

        public ObservableCollection<GeoStatisticViewModel> Items { get; private set; }
    }
}
