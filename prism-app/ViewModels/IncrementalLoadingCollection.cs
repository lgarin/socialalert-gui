using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Socialalert.ViewModels
{
    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        private readonly int itemsPerPage;
        private bool hasMoreItems;
        private volatile int currentPage;
        private readonly LoadItemsDelegate loadItemsDelegate;

        public delegate Task<IEnumerable<T>> LoadItemsDelegate(int pageIndex, int pageSize);

        public IncrementalLoadingCollection(LoadItemsDelegate loadItemsDelegate = null, int itemsPerPage = Constants.ItemsPerPage)
        {
            this.loadItemsDelegate = loadItemsDelegate;
            this.itemsPerPage = itemsPerPage;
            this.hasMoreItems = loadItemsDelegate != null;
        }

        private void AddLoadedItems(IEnumerable<T> items, int pageNumber)
        {
            if (pageNumber != currentPage)
            {
                // several threads tried to load more data at the same time
                return;
            }

            currentPage++;

            foreach (T item in items)
            {
                this.Add(item);
            }

            if (items.Count() < itemsPerPage)
            {
                hasMoreItems = false;
                OnPropertyChanged(new PropertyChangedEventArgs("HasMoreItems"));
            }
        }

        public bool HasMoreItems
        {
            get { return hasMoreItems; }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c));
        }

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c)
        {
            int pageNumber = currentPage;
            var result = await loadItemsDelegate(pageNumber, itemsPerPage);
            int resultCount = result.Count();

            // update state on the dispatcher thread
            await Window.Current.Dispatcher.RunAsync(
                   CoreDispatcherPriority.Normal,
                   () => AddLoadedItems(result, pageNumber));

            return new LoadMoreItemsResult() { Count = (uint) resultCount };
        }
    }
}
