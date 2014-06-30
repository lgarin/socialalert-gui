using Microsoft.Practices.Prism.StoreApps;
using Socialalert.ViewModels;

namespace Socialalert.Views
{
    public sealed partial class MapStatisticPage : VisualStateAwarePage
    {
        public MapStatisticPage()
        {
            this.InitializeComponent();
        }

        public void map_ViewChangeEnded(object sender, Bing.Maps.ViewChangeEndedEventArgs e)
        {
            var vm = DataContext as MapStatisticPageViewModel;
            var bounds = map.Bounds;
            if (vm.MapViewChangedCommand.CanExecute(bounds))
            {
                vm.MapViewChangedCommand.Execute(bounds);
            }
        }
    }
}
