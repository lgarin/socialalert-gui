using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class ObservableCollectionView<T> : ObservableCollection<T>
    {
        private readonly Predicate<T> filter;
        private readonly ObservableCollection<T> source;
        
        public ObservableCollectionView(ObservableCollection<T> source, Predicate<T> filter)
        {
            this.filter = filter;
            this.source = source;
            source.CollectionChanged += SourceCollectionChanged;
            Fill();
        }

        private void Fill()
        {
            Clear();
            foreach (T item in source)
            {
                if (filter(item))
                    Add(item);
            }
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    HandleAddedItems(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Move:

                    break;

                case NotifyCollectionChangedAction.Remove:
                    HandleRemovedItems(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    HandleReplacedItems(e.OldItems, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Fill();
                    break;
            }
        }

        private void HandleReplacedItems(IList oldItems, IList newItems)
        {
            for (int index = 0; index < oldItems.Count; index++)
            {
                T item = (T)oldItems[index];
                if (filter(item))
                {
                    int foundIndex = IndexOf(item);
                    if (foundIndex != -1)
                        this[foundIndex] = (T)newItems[index];
                }
            }
        }

        private void HandleRemovedItems(IList oldItems)
        {
            foreach (T item in oldItems)
            {
                if (filter(item))
                    Remove(item);
            }
        }

        private void HandleAddedItems(IList newItems)
        {
            foreach (T item in newItems)
            {
                if (filter(item))
                    Add(item);
            }
        }

    }
}
