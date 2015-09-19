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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Socialalert.ViewModels
{

    public class UploadImagePageViewModel : LoadableViewModel
    {
        private bool video;
        private Uri mediaUri;
        
        private string title;
        private string tags;
        private GeoAddress location;

        private BitmapImage bitmapImage;
        private MediaElement mediaElement;

        private MediaCategory? selectedCategory;


        [Dependency]
        protected ImageChooserService ImageChooser { get; set; }

        [Dependency]
        protected IMediaUploadService MediaUploadService { get; set; }

        [Dependency]
        protected IApplicationStateService ApplicationStateService { get; set; }

        [Dependency]
        protected IGeoLocationService GeoLocationService { get; set; }

        public MediaElement MediaElement
        {
            get
            {
                return mediaElement;
            }
            set
            {
                SetProperty(ref mediaElement, value);
            }
        }

        public BitmapImage BitmapImage
        {
            get
            {
                return bitmapImage;
            }
            set
            {
                SetProperty(ref bitmapImage, value);
            }
        }

        public DelegateCommand UploadMediaCommand { get; private set; }

        public DelegateCommand PostCommand { get; private set; }

        public DelegateCommand<LocationRect> MapViewChangedCommand { get; private set; }

        public LocationRect MapBounds
        {
            get
            {
                if (location != null && location.Latitude != null && location.Longitude != null)
                {
                    return new LocationRect(new Location(location.Latitude.Value, location.Longitude.Value), 1.0, 1.0);
                }
                return null;
            }
        }

        public bool IsVideo
        {
            get
            {
                return video;
            }
            set
            {
                SetProperty(ref video, value);
                if (video)
                {
                    BitmapImage.UriSource = null;
                }
                else
                {
                    MediaElement.Source = null;
                }
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
            UploadMediaCommand = new DelegateCommand(UploadMedia, CanUploadMedia);
            ApplicationStateService.PropertyChanged += (s, e) => UploadMediaCommand.RaiseCanExecuteChanged();
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

        private void ChangeLocation(GeoAddress address)
        {
            location = address;
            OnPropertyChanged(() => MapBounds);
        }

        private bool CanUploadMedia()
        {
            return ApplicationStateService.CurrentUser != null;
        }

        private void UploadMedia()
        {
            if (IsVideo)
            {
                UploadVideo();
            }
            else
            {
                UploadPicture();
            }
        }

        private async void UploadPicture()
        {
            var file = await ImageChooser.LoadImage();
            if (file == null)
            {
                return;
            }

            ResetUpload();
            try
            {
                LoadingData = true;
                var info = await MediaUploadService.PostPictureAsync(file);
                BitmapImage.UriSource = HandleUploadInfo(info);
                OnPropertyChanged(() => BitmapImage);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }


        private async void UploadVideo()
        {
            var file = await ImageChooser.LoadVideo();
            if (file == null)
            {
                return;
            }

            ResetUpload();
            try
            {
                LoadingData = true;
                var info = await MediaUploadService.PostVideoAsync(file);
                MediaElement.Source = HandleUploadInfo(info);
                OnPropertyChanged(() => MediaElement);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        private Uri HandleUploadInfo(MediaUploadInfo info)
        {
            mediaUri = info.Uri;
            ChangeLocation(info.Location);
            var baseUri = new Uri(ResourceDictionary["BasePreviewUrl"] as string, UriKind.Absolute);
            return new Uri(baseUri, mediaUri);
        }

        private void CompleteLoad()
        {
            LoadingData = false;
            PostCommand.RaiseCanExecuteChanged();
        }

        private void ResetUpload()
        {
            LoadingData = false;
            mediaUri = null;
            MediaElement.Source = null;
            BitmapImage.UriSource = null;
        }

        private async void ShowError(String technicalMessage)
        {

            ResetUpload();
            var errorMessage = string.Format(CultureInfo.CurrentCulture,
                                                 ResourceLoader.GetString("GeneralServiceErrorMessage"),
                                                 Environment.NewLine, technicalMessage);
            await AlertMessageService.ShowAsync(errorMessage, ResourceLoader.GetString("ErrorServiceUnreachable"));
        }

        private bool CanPostPicture()
        {
            return ApplicationStateService.CurrentUser != null && !string.IsNullOrWhiteSpace(Title) && mediaUri != null && SelectedCategory != null;
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
                if (IsVideo)
                {
                    await ExecuteAsync(new ClaimVideoRequest() { VideoUri = mediaUri, Title = this.Title, Tags = this.TagArray, Categories = this.CategoryArray, Location = location });
                }
                else
                {
                    await ExecuteAsync(new ClaimPictureRequest() { PictureUri = mediaUri, Title = this.Title, Tags = this.TagArray, Categories = this.CategoryArray, Location = location });
                }
                
                NavigationService.GoBack();
            }
            catch (JsonRpcException e)
            {
                await AlertMessageService.ShowAsync(e.Message, ResourceLoader.GetString("ErrorCannotPost"));
            }
        }

        public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            MediaElement = new MediaElement() { IsMuted = true, IsLooping = true, AutoPlay = true, Stretch = Stretch.Uniform };
            MediaElement.MediaFailed += (s, e) => ShowError(e.ErrorMessage);
            MediaElement.MediaOpened += (s, e) => CompleteLoad();

            BitmapImage = new BitmapImage();
            BitmapImage.ImageFailed += (s, e) => ShowError(e.ErrorMessage);
            BitmapImage.ImageOpened += (s, e) => CompleteLoad();

            ResetUpload();
            ChangeLocation(await GeoLocationService.GetCurrentLocation(PositionAccuracy.High));
        }
    }
}

