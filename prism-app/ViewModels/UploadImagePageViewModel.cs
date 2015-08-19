using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Socialalert.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Socialalert.ViewModels
{
    public class UploadImagePageViewModel : LoadableViewModel
    {
        private BitmapImage uploadedPicture;

        [Dependency]
        protected ImageChooserService ImageChooser {get; set;}

        [Dependency]
        protected IImageUploadService ImageUploadService { get; set; }

        [Dependency]
        protected IApplicationStateService ApplicationStateService { get; set; }

        public DelegateCommand UploadPictureCommand { get; private set; }

        public UploadImagePageViewModel(IApplicationStateService applicationStateService)
        {
            ApplicationStateService = applicationStateService;
            UploadPictureCommand = new DelegateCommand(UploadPicture, CanUploadPicture);
            ApplicationStateService.PropertyChanged += (s, e) => UploadPictureCommand.RaiseCanExecuteChanged();
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
            Exception exception = null;
            var baseUri = new Uri(ResourceDictionary["BasePreviewUrl"] as string, UriKind.Absolute);
            try
            {
                LoadingData = true;
                var relativeUri = await ImageUploadService.PostPictureAsync(stream);
                UploadedPicture = new BitmapImage(new Uri(baseUri, relativeUri));
            }
            catch (Exception e)
            {
                exception = e;
                
            }
            finally
            {
                LoadingData = false;
            }

            if (exception != null)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture,
                                                 ResourceLoader.GetString("GeneralServiceErrorMessage"),
                                                 Environment.NewLine, exception.Message);
                await AlertMessageService.ShowAsync(errorMessage, ResourceLoader.GetString("ErrorServiceUnreachable"));
            }
        }
    }
}
