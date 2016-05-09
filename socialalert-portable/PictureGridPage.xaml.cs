using Bravson.Socialalert.Portable.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public partial class PictureGridPage : ModelPage
    {
        public PictureGridPage()
        {
            InitializeComponent();
            DoRefresh();
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
            foreach (var media in result.Content)
            {
                ItemList.Add(new PictureGridItem(media, BaseThumbnailUri));
            }
            BatchCommit();
            listView.EndRefresh();
            Refresh.ChangeCanExecute();
        }

        private bool CanRefresh()
        {
            return !IsBusy && listView != null && !listView.IsRefreshing;
        }
    }
}
