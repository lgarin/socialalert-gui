﻿using Bravson.Socialalert.Portable.Model;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using System.Collections.Generic;
using Bravson.Socialalert.Portable.Util;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Threading.Tasks;

namespace Bravson.Socialalert.Portable
{
    public partial class PictureGridPage : ModelPage
    {
        private int columnCount;

        public PictureGridPage()
        {
            InitializeComponent();
            SizeChanged += OnPageSizeChanged;
            plusButton.Clicked += OnPlusButtonClicked;
        }

        private void OnPlusButtonClicked(object sender, EventArgs e)
        {
            cameraButton.IsVisible = !cameraButton.IsVisible;
            mapButton.IsVisible = !mapButton.IsVisible;
        }

        public ObservableCollection<PictureGridItem> ItemList { get; } = new ObservableCollection<PictureGridItem>();

        public Uri BaseThumbnailUri { get; } = new Uri(App.Current.Resources["BaseThumbnailUrl"] as string, UriKind.Absolute);

        public Command Refresh
        {
            get { return Command(DoRefresh, CanRefresh); }
        }

        public Command Search
        {
            get { return Command(DoSearch, CanSearch); }
        }

        public Command CapturePhoto
        {
            get { return Command(DoCapturePhoto, CanCapturePhoto); }
        }

        private async void DoCapturePhoto()
        {
            var options = new StoreCameraMediaOptions() { PhotoSize = PhotoSize.Large, SaveToAlbum = false };
            var photoAsync = CrossMedia.Current.TakePhotoAsync(options);
            var location = default(Position);
            if (CrossGeolocator.Current.IsGeolocationEnabled)
            { 
                CrossGeolocator.Current.DesiredAccuracy = 20;
                location = await CrossGeolocator.Current.GetPositionAsync();
            }
            var photo = await photoAsync;
            if (photo != null)
            {
                using (photo)
                {
                    PendingUpload upload = new PendingUpload(MediaType.PICTURE, photo.Path);
                    await App.DatabaseConnection.UpsertPendingUpload(upload);
                    
                    App.Notification.ShowUpload(upload);
                    // TODO navigate
                }
            }
        }

        private bool CanCapturePhoto()
        {
            return CrossMedia.Current.IsTakePhotoSupported;
        }

        public string Keywords
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        private void DoSearch()
        {
            DoRefresh();
        }

        private bool CanSearch()
        {
            return true;
        }

        private async void DoRefresh()
        {
            if (!listView.IsRefreshing)
            {
                listView.BeginRefresh();
            }
            var result = await ExecuteAsync(new SearchMediaRequest { PageNumber=0, PageSize=100, MaxAge=2000*Constants.MillisPerDay, Keywords=Keywords });
            BatchBegin();
            ItemList.Clear();
            foreach (var mediaGroup in result.Content.Batch(columnCount))
            {
                var padding = Enumerable.Repeat(default(MediaInfo), columnCount - mediaGroup.Count());
                ItemList.Add(new PictureGridItem(mediaGroup.Union(padding), BaseThumbnailUri));
            }
            BatchCommit();
            listView.EndRefresh();
            Refresh.ChangeCanExecute();
        }

        private bool CanRefresh()
        {
            return !IsBusy && listView != null && !listView.IsRefreshing;
        }

        void OnPageSizeChanged(object sender, EventArgs args)
        {
            BatchBegin();
            ItemList.Clear();

            // Portrait mode. 
            if (Width < Height)
            {
                columnCount = 3;
                listView.ItemTemplate = Resources["template3Columns"] as DataTemplate;
            }
            // Landscape mode. 
            else
            {
                columnCount = 5;
                listView.ItemTemplate = Resources["template5Columns"] as DataTemplate;
            }

            BatchCommit();
            DoRefresh();
        }
    }
}