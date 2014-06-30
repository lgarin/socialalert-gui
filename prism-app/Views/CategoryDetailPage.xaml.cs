using Microsoft.Practices.Prism.StoreApps;
using Socialalert.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace Socialalert.Views
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class CategoryDetailPage : VisualStateAwarePage
    {
        public CategoryDetailPage()
        {
            this.InitializeComponent();
        }

        public void map_ViewChangeEnded(object sender, Bing.Maps.ViewChangeEndedEventArgs e)
        {
            var vm = DataContext as CategoryDetailPageViewModel;
            var bounds = map.Bounds;
            if (vm.MapViewChangedCommand.CanExecute(bounds))
            {
                vm.MapViewChangedCommand.Execute(bounds);
            }
        }
    }
}