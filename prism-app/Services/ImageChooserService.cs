using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Socialalert.Services
{
    public class ImageChooserService
    {
        public async Task<IRandomAccessStream> LoadImage()
        {
            return await LoadFile(PickerLocationId.PicturesLibrary);
        }

        public async Task<IRandomAccessStream> LoadVideo()
        {
            return await LoadFile(PickerLocationId.VideosLibrary);
        }

        private async Task<IRandomAccessStream> LoadFile(PickerLocationId locationId)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types.
            openPicker.FileTypeFilter.Clear();
            if (locationId == PickerLocationId.PicturesLibrary)
            {
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".jpg");
            }
            else if (locationId == PickerLocationId.VideosLibrary)
            {
                openPicker.FileTypeFilter.Add(".mov");
                openPicker.FileTypeFilter.Add(".mp4");
            }

            // Open the file picker.
            StorageFile file = await openPicker.PickSingleFileAsync();

            // file is null if user cancels the file picker.
            if (file != null)
            {
                // Open a stream for the selected file.
                return await file.OpenAsync(FileAccessMode.Read);
            }

            return null;
        }
    }
}
