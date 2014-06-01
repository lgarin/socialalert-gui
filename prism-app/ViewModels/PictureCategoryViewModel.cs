using Bing.Maps;
using Microsoft.Practices.Prism.StoreApps;
using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public sealed class PictureCategoryViewModel : SimpleViewModel
    {

        public PictureCategoryViewModel(String name, ObservableCollection<PictureViewModel> items, DelegateCommand<PictureCategoryViewModel> selectedCommand)
        {
            Items = new IncrementalLoadingCollection<PictureViewModel>();
            GeoLocatedItems = new ObservableCollection<PictureViewModel>();
            Id = name;
            Title = name; // TODO translate with resources
            Items = items;
            CategorySelectedCommand = selectedCommand;
            PropertyChanged += (s, e) => { CategorySelectedCommand.RaiseCanExecuteChanged(); };
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GeoLocatedItems.Clear();
            foreach (var picture in Items.Where(p => p.HasGeoLocation))
            {
                GeoLocatedItems.Add(picture);
            }
        }

        public DelegateCommand<PictureCategoryViewModel> CategorySelectedCommand { get; private set; }

        public string Id { get { return Get<string>(); } set { Set(value); } }
        public string Title { get { return Get<string>(); } set { Set(value); } }
        public ObservableCollection<PictureViewModel> Items { get; private set; }
        public ObservableCollection<PictureViewModel> GeoLocatedItems { get; private set; }
    }
}
