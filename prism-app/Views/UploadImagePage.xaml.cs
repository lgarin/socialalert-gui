using Microsoft.Practices.Prism.StoreApps;
using Socialalert.ViewModels;

namespace Socialalert.Views
{
    public sealed partial class UploadImagePage : VisualStateAwarePage
    {
        public UploadImagePage()
        {
            this.InitializeComponent();
        }

        public void map_ViewChangeEnded(object sender, Bing.Maps.ViewChangeEndedEventArgs e)
        {
            var vm = DataContext as UploadImagePageViewModel;
            var bounds = map.Bounds;
            if (vm.MapViewChangedCommand.CanExecute(bounds))
            {
                vm.MapViewChangedCommand.Execute(bounds);
            }
        }
    }
}
