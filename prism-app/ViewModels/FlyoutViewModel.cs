using Microsoft.Practices.Prism.StoreApps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public abstract class FlyoutViewModel : SimpleViewModel
    {

        protected FlyoutViewModel()
        {
            CancelCommand = new DelegateCommand(Hide);
        }

        protected void Hide()
        {
            IsFlyoutClosed = true;
        }

        public bool IsFlyoutClosed
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set<bool>(value);
                if (value)
                {
                    IsFlyoutClosed = false;
                }
            }
        }

        public DelegateCommand CancelCommand { get; private set; }

        public abstract void Reset();
    }
}
