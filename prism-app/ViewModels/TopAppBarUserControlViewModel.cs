using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class TopAppBarUserControlViewModel : ViewModel
    {
        [Dependency]
        public IUnityContainer Container {get; set;}
        
        public DelegateCommand DumpDataCommand { get; private set; }

        public TopAppBarUserControlViewModel()
        {
            DumpDataCommand = new DelegateCommand(DumpData);
        }

        private void DumpData()
        {
            var viewModel = Container.Resolve<PictureCommentUserControlViewModel>();
            string json = viewModel.SerializeToJson();
            Debug.WriteLine(viewModel.GetType().Name);
            Debug.WriteLine(json);
            Debug.WriteLine("----------------------------------");
        }

    }
}
