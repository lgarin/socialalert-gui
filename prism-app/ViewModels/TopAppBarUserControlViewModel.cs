using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class DumpDataUserControlEvent : PubSubEvent<string>
    {

    }

    public class TopAppBarUserControlViewModel : ViewModel
    {
        [Dependency]
        protected IEventAggregator EventAggregator { get; set; }

        public DelegateCommand DumpDataCommand { get; private set; }

        public TopAppBarUserControlViewModel()
        {
            DumpDataCommand = new DelegateCommand(PublishDumpDataEvent);
        }

        private void PublishDumpDataEvent()
        {
            EventAggregator.GetEvent<DumpDataUserControlEvent>().Publish(null);
        }
    }
}
