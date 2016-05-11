using Bravson.Socialalert.Portable.Model;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using System.Collections.Generic;
using Bravson.Socialalert.Portable.Util;

namespace Bravson.Socialalert.Portable
{
    public partial class PictureGridPage : ModelPage
    {
        private int columnCount;

        public PictureGridPage()
        {
            InitializeComponent();
            SizeChanged += OnPageSizeChanged;
            //DoRefresh();
        }

        public ObservableCollection<PictureGridItem> ItemList { get; } = new ObservableCollection<PictureGridItem>();

        public Uri BaseThumbnailUri { get; } = new Uri(App.Current.Resources["BaseThumbnailUrl"] as string, UriKind.Absolute);

        public Command Refresh
        {
            get { return Command(DoRefresh, CanRefresh); }
        }

        private async void DoRefresh()
        {
            var result = await ExecuteAsync(new SearchMediaRequest { PageNumber=0, PageSize=100, MaxAge=2000*Constants.MillisPerDay });
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
