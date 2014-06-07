using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public class EventAggregator<T> where T : EventArgs
    {
        private readonly Action handler;
        private readonly TimeSpan delay;
        private bool trigger;

        public EventAggregator(TimeSpan delay, Action handler)
        {
            this.delay = delay;
            this.handler = handler;
        }

        public async void HandleEvent(object sender, T arg)
        {
            trigger = true;
            await Task.Delay(delay);
            if (trigger)
            {
                trigger = false;
                handler();

            }
            else
            {
                Debug.WriteLine("Skipped event " + arg);
            }
        }
    }
}
