using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    /**
     * use regex
     * public (\w+\??) (\w+);
     * public $1 $2 { get { return Get<$1>(); } set { Set(value); } }
     * 
     * public (\w+\??)\[\] (\w+);
     * public ObservableCollection<$1> $2 = new ObservableCollection<$1>();
     */
    public abstract class SimpleModel : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> propertyDictionary = new Dictionary<string, object>();
        private readonly Dictionary<string, Command> commandDictionary = new Dictionary<string, Command>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected ICommand Command(Action action, Func<bool> check, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            Command value;
            if (commandDictionary.TryGetValue(propertyName, out value))
            {
                return value;
            }

            value = new Command(action, check);
            commandDictionary.Add(propertyName, value);
            return value;
        }

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            object value;
            if (propertyDictionary.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        protected bool Set<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            T oldValue = Get<T>(propertyName);
            if (EqualityComparer<T>.Default.Equals(newValue, oldValue))
            {
                return false;
            }
            propertyDictionary[propertyName] = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            foreach (var command in commandDictionary.Values)
            {
                command.ChangeCanExecute();
            }
            return true;
        }
    }
}
