using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int currentPage;
        private readonly LoadItemsDelegate loadItemsDelegate;

        public delegate Task<IEnumerable<T>> LoadItemsDelegate(int pageIndex, int pageSize);

        public IncrementalLoadingCollection(LoadItemsDelegate loadItemsDelegate = null, int itemsPerPage = Constants.ItemsPerPage)
        {
            this.loadItemsDelegate = loadItemsDelegate;
            this.itemsPerPage = itemsPerPage;
            this.hasMoreItems = loadItemsDelegate != null;
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            { 
                this.Add(item);
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

        public async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c)
        {
            var result = await loadItemsDelegate(currentPage++, itemsPerPage);
            int resultCount = result.Count();

            if (resultCount == 0)
            {
                hasMoreItems = false;
            }
            else
            {
                await Window.Current.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => AddRange(result));
            }

            return new LoadMoreItemsResult() { Count = (uint) resultCount };
        }
    }
}
