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
        private ObservableCollection<PictureViewModel> items;
        private ObservableCollectionView<PictureViewModel> geoLocatedItems;

        public PictureCategoryViewModel(String name, DelegateCommand<PictureCategoryViewModel> selectedCommand, ObservableCollection<PictureViewModel> items = null)
        {
            Id = name;
            Title = name; // TODO translate with resources
            CategorySelectedCommand = selectedCommand;
            PropertyChanged += (s, e) => { CategorySelectedCommand.RaiseCanExecuteChanged(); };
            Items = items;
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
        public ObservableCollection<PictureViewModel> Items { 
            get {
                return items;
            }
            set {
                SetProperty(ref items, value);
                if (items != null)
                {
                    GeoLocatedItems = new ObservableCollectionView<PictureViewModel>(items, i => i.HasGeoLocation);
                }
                else
                {
                    GeoLocatedItems = null;
                }
            }
        }

        public ObservableCollectionView<PictureViewModel> GeoLocatedItems { get { return geoLocatedItems; } private set { SetProperty(ref geoLocatedItems, value); } }
    }
}
