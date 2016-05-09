using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable.Model
{
    public sealed class PropertyStore
    {
        private readonly Dictionary<string, object> propertyDictionary = new Dictionary<string, object>();
        
        public T GetValue<T>(string propertyName = null)
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

        public bool SetValue<T>(T newValue, string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            T oldValue = GetValue<T>(propertyName);
            if (EqualityComparer<T>.Default.Equals(newValue, oldValue))
            {
                return false;
            }
            propertyDictionary[propertyName] = newValue;
            return true;
        }
    }

    public sealed class CommandStore
    {
        private readonly Dictionary<string, Command> commandDictionary = new Dictionary<string, Command>();

        public Command GetOrCreate(Action action, Func<bool> check, string propertyName)
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

        public void RefreshAll()
        {
            foreach (var command in commandDictionary.Values)
            {
                command.ChangeCanExecute();
            }
        }
    }

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
        private PropertyStore properties = new PropertyStore();

        public event PropertyChangedEventHandler PropertyChanged;

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            return properties.GetValue<T>(propertyName);
        }

        protected bool Set<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (properties.SetValue(newValue, propertyName)) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }
}
