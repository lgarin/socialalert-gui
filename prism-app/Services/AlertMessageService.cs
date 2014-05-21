using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Socialalert.Services
{
    public sealed class DialogCommand
    {
        public object Id;
        public string Label;
        public Action Invoked;
    }

    public interface IAlertMessageService
    {
        Task ShowAsync(string message, string title);

        Task ShowAsync(string message, string title, IEnumerable<DialogCommand> dialogCommands);
    }

    public sealed class AlertMessageService : IAlertMessageService
    {
        private static bool _isShowing = false;

        public async Task ShowAsync(string message, string title)
        {
            await ShowAsync(message, title, null);
        }

        public async Task ShowAsync(string message, string title, IEnumerable<DialogCommand> dialogCommands)
        {
            // Only show one dialog at a time.
            if (!_isShowing)
            {
                var messageDialog = new MessageDialog(message, title);

                if (dialogCommands != null)
                {
                    var commands = dialogCommands.Select(c => new UICommand(c.Label, (command) => c.Invoked(), c.Id));

                    foreach (var command in commands)
                    {
                        messageDialog.Commands.Add(command);
                    }
                }

                _isShowing = true;
                await messageDialog.ShowAsync();
                _isShowing = false;
            }
        }
    }
}
