using Bravson.Socialalert.Portable.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public partial class PictureMetadataPage : ModelPage
    {
        private PendingUpload upload;

        public PictureMetadataPage(PendingUpload upload)
        {
            this.upload = upload;
            MediaDescription = upload.Description;
            MediaTitle = upload.Title;
            MediaSource = (FileImageSource) upload.FilePath;
            InitializeComponent();
        }

        public string MediaTitle
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public string MediaDescription
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public ImageSource MediaSource
        {
            get; private set;
        }

        public Command SaveCommand
        {
            get { return Command(OnSave, CanSave); }
        }

        private void OnSave()
        {
            upload.Title = MediaTitle;
            upload.Description = MediaDescription;
            try
            {
                App.UploadService.SaveMetadata(upload);
                App.Current.MainPage = new PictureGridPage();
            }
            catch (Exception e)
            {
                DisplayError("Save failed", e);
            }
            SaveCommand.ChangeCanExecute();
        }

        private bool CanSave()
        {
            return MediaTitle != null && MediaTitle != upload.Title;
        }
    }
}
