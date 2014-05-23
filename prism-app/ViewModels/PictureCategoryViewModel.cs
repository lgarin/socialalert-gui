using Microsoft.Practices.Prism.StoreApps;
using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class PictureCategoryViewModel : SimpleViewModel
    {
        public PictureCategoryViewModel(String name, Uri basePictureUri, IList<PictureInfo> data, DelegateCommand<PictureCategoryViewModel> selectedCommand)
        {
            Items = new ObservableCollection<PictureViewModel>();
            Id = name;
            Title = name; // TODO translate with resources
            foreach (var picture in data)
            {
                Items.Add(new PictureViewModel(basePictureUri, picture));
            }
            CategorySelectedCommand = selectedCommand;
        }

        public DelegateCommand<PictureCategoryViewModel> CategorySelectedCommand { get; private set; }

        public string Id { get { return Get<string>(); } set { Set(value); } }
        public string Title { get { return Get<string>(); } set { Set(value); } }
        public ObservableCollection<PictureViewModel> Items { get; private set; }
    }
}
