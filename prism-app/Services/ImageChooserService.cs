﻿using System;
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
        public async Task<StorageFile> LoadImage()
        {
            return await LoadFile(PickerLocationId.PicturesLibrary);
        }

        public async Task<StorageFile> LoadVideo()
        {
            return await LoadFile(PickerLocationId.VideosLibrary);
        }

        private async Task<StorageFile> LoadFile(PickerLocationId locationId)
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

            return file;
        }
    }
}
