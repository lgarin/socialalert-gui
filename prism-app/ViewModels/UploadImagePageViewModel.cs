using Bing.Maps;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Socialalert.ViewModels
{

    public class UploadImagePageViewModel : LoadableViewModel
    {
        private Uri pictureUri;
        private BitmapImage uploadedPicture;
        private string title;
        private string tags;
        private GeoAddress location;


        private MediaCategory? selectedCategory;


        [Dependency]
        protected ImageChooserService ImageChooser { get; set; }

        [Dependency]
        protected IImageUploadService ImageUploadService { get; set; }

        [Dependency]
        protected IApplicationStateService ApplicationStateService { get; set; }

        [Dependency]
        protected IGeoLocationService GeoLocationService { get; set; }

        public DelegateCommand UploadPictureCommand { get; private set; }

        public DelegateCommand PostCommand { get; private set; }

        public DelegateCommand<LocationRect> MapViewChangedCommand { get; private set; }

        public LocationRect MapBounds
        {
            get
            {
                if (location != null && location.Latitude != null && location.Longitude != null)
                {
                    return new LocationRect(new Location(location.Latitude.Value, location.Longitude.Value), 10.0, 10.0);
                }
                return null;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value);
                PostCommand.RaiseCanExecuteChanged();
            }
        }

        public string Tags
        {
            get
            {
                return tags;
            }
            set
            {
                SetProperty(ref tags, value);
            }
        }

        public MediaCategory[] Categories
        {
            get
            {

                return (MediaCategory[])Enum.GetValues(typeof(MediaCategory));
            }
        }

        public UploadImagePageViewModel(IApplicationStateService applicationStateService)
        {
            ApplicationStateService = applicationStateService;
            UploadPictureCommand = new DelegateCommand(UploadPicture, CanUploadPicture);
            ApplicationStateService.PropertyChanged += (s, e) => UploadPictureCommand.RaiseCanExecuteChanged();
            PostCommand = new DelegateCommand(PostPicture, CanPostPicture);
            ApplicationStateService.PropertyChanged += (s, e) => PostCommand.RaiseCanExecuteChanged();
            MapViewChangedCommand = new DelegateCommand<LocationRect>(ChangeLocation);
        }

        private void ChangeLocation(LocationRect box)
        {
            location = new GeoAddress();
            location.Longitude = box.Center.Longitude;
            location.Latitude = box.Center.Latitude;
        }

        public BitmapImage UploadedPicture
        {
            get { return uploadedPicture; }
            private set { SetProperty(ref uploadedPicture, value); }
        }

        private bool CanUploadPicture()
        {
            return ApplicationStateService.CurrentUser != null;
        }

        private async void UploadPicture()
        {
            var stream = await ImageChooser.LoadImage();
            if (stream == null)
            {
                return;
            }

            try
            {
                LoadingData = true;
                pictureUri = await ImageUploadService.PostPictureAsync(stream);
                var baseUri = new Uri(ResourceDictionary["BasePreviewUrl"] as string, UriKind.Absolute);
                UploadedPicture = new BitmapImage(new Uri(baseUri, pictureUri));
                UploadedPicture.ImageFailed += (s, e) => ShowError(e.ErrorMessage);
                UploadedPicture.ImageOpened += (s, e) => CompleteLoad();
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        private void CompleteLoad()
        {
            LoadingData = false;
            PostCommand.RaiseCanExecuteChanged();
        }

        private async void ShowError(String technicalMessage)
        {
            LoadingData = false;
            UploadedPicture = null;
            pictureUri = null;

            var errorMessage = string.Format(CultureInfo.CurrentCulture,
                                                 ResourceLoader.GetString("GeneralServiceErrorMessage"),
                                                 Environment.NewLine, technicalMessage);
            await AlertMessageService.ShowAsync(errorMessage, ResourceLoader.GetString("ErrorServiceUnreachable"));
        }

        private bool CanPostPicture()
        {
            return ApplicationStateService.CurrentUser != null && !string.IsNullOrWhiteSpace(Title) && UploadedPicture != null && SelectedCategory != null;
        }

        private string[] TagArray
        {
            get
            {
                return Tags != null ? Tags.Split(' ') : new string[0];
            }
        }

        public MediaCategory? SelectedCategory
        {
            get { return selectedCategory; }
            set { SetProperty(ref selectedCategory, value); PostCommand.RaiseCanExecuteChanged(); }
        }


        private MediaCategory[] CategoryArray
        {
            get
            {
                if (SelectedCategory == null)
                {
                    return new MediaCategory[0];
                }
                return new[] { SelectedCategory.Value };
            }
        }

        private async void PostPicture()
        {
            try
            {
                
                await ExecuteAsync(new ClaimPictureRequest() { PictureUri = pictureUri, Title = this.Title, Tags = this.TagArray, Categories = this.CategoryArray, Location = location });
                NavigationService.GoBack();
            }
            catch (JsonRpcException e)
            {
                await AlertMessageService.ShowAsync(e.Message, ResourceLoader.GetString("ErrorCannotPost"));
            }
        }

        public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            location = await GeoLocationService.GetCurrentLocation(PositionAccuracy.High);
        }
    }
}
