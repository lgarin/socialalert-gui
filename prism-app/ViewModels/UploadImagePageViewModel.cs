using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using Socialalert.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
        private ObservableCollection<MediaCategory> selectedCategories = new ObservableCollection<MediaCategory>();


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

        public ObservableCollection<MediaCategory> SelectedCategories
        {
            get
            {
                return selectedCategories;
            }
            set
            {
                SetProperty(ref selectedCategories, value);
            }
        }

        public UploadImagePageViewModel(IApplicationStateService applicationStateService)
        {
            ApplicationStateService = applicationStateService;
            UploadPictureCommand = new DelegateCommand(UploadPicture, CanUploadPicture);
            ApplicationStateService.PropertyChanged += (s, e) => UploadPictureCommand.RaiseCanExecuteChanged();
            PostCommand = new DelegateCommand(PostPicture, CanPostPicture);
            ApplicationStateService.PropertyChanged += (s, e) => PostCommand.RaiseCanExecuteChanged();
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
            return ApplicationStateService.CurrentUser != null && !string.IsNullOrWhiteSpace(Title) && UploadedPicture != null;
        }

        private string[] TagArray
        {
            get
            {
                return Tags != null ? Tags.Split(' ') : new string[0];
            }
        }

        private MediaCategory[] CategoryArray
        {
            get
            {
                /*
                MediaCategory[] array = new MediaCategory[SelectedCategories.Count];
                SelectedCategories.CopyTo(array, 0);
                return array;
                */
                return new[] { MediaCategory.AWESOME };
            }
        }

        private async void PostPicture()
        {
            try
            {
                var location = await GeoLocationService.GetCurrentLocation(PositionAccuracy.High);
                await ExecuteAsync(new ClaimPictureRequest() { PictureUri = pictureUri, Title = this.Title, Tags = this.TagArray, Categories = this.CategoryArray, Location = location });
                NavigationService.GoBack();
            }
            catch (JsonRpcException e)
            {
                await AlertMessageService.ShowAsync(e.Message, ResourceLoader.GetString("ErrorCannotPost"));
            }
        }
    }
}
