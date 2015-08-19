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
        private readonly ObservableCollection<PictureViewModel> items = new ObservableCollection<PictureViewModel>();
        private readonly ObservableCollection<PictureViewModel> geoLocatedItems = new ObservableCollection<PictureViewModel>();

        public PictureCategoryViewModel(String name, DelegateCommand<PictureCategoryViewModel> selectedCommand, IEnumerable<PictureViewModel> items = null)
        {
            Id = name;
            Title = name; // TODO translate with resources
            CategorySelectedCommand = selectedCommand;
            PropertyChanged += (s, e) => { CategorySelectedCommand.RaiseCanExecuteChanged(); };
            if (items != null)
            {
                SetItems(items);
            }
        }

        public void SetItems(IEnumerable<PictureViewModel> items)
        {
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
            GeoLocatedItems.Clear();
            foreach (var item in items.Where(p => p.HasGeoLocation).Reverse())
            {
                GeoLocatedItems.Add(item);
            }
        }

        public DelegateCommand<PictureCategoryViewModel> CategorySelectedCommand { get; private set; }

        public string Id { get { return Get<string>(); } set { Set(value); } }
        public string Title { get { return Get<string>(); } set { Set(value); } }
        public ObservableCollection<PictureViewModel> Items { get { return items; } }
        public ObservableCollection<PictureViewModel> GeoLocatedItems { get { return geoLocatedItems; } }
    }
}
