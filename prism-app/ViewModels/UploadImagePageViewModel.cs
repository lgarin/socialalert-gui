using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using Socialalert.Services;
using System.ComponentModel.DataAnnotations;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Socialalert.ViewModels
{
    public class UploadImagePageViewModel : LoadableViewModel
    {
        private BitmapImage uploadedPicture;

        [Dependency]
        protected ImageChooserService ImageChooser {get; set;}

        public DelegateCommand UploadPictureCommand { get; private set; }

        public BitmapImage UploadedPicture
        {
            get { return uploadedPicture; }
            private set { SetProperty(ref uploadedPicture, value); }
        }

        public UploadImagePageViewModel()
        {
            UploadPictureCommand = new DelegateCommand(UploadPicture, CanUploadPicture);
        }

        private bool CanUploadPicture()
        {
            return true;
        }

        private async void UploadPicture()
        {
            UploadedPicture = await ImageChooser.LoadImage();
        }
    }
}
