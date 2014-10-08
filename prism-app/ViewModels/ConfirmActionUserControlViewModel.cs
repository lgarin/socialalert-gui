using Microsoft.Practices.Prism.StoreApps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class ConfirmActionUserControlViewModel : FlyoutViewModel
    {
        public ConfirmActionUserControlViewModel(string message, DelegateCommand command) 
        {
            ContinueCommand = new DelegateCommand(() => { Hide(); command.Execute(); });
            ShowCommand = new DelegateCommand(Reset, () => command.CanExecute());
            command.CanExecuteChanged += (s, e) => ShowCommand.RaiseCanExecuteChanged();
            Message = message;
        }

        public DelegateCommand ShowCommand { get; private set; }

        public string Message
        {
            get
            {
                return Get<string>();
            }
            set
            {
                Set<string>(value);
            }
        }

        public override void Reset()
        {
            
        }

        public DelegateCommand ContinueCommand { get; private set; }
    }
}
