using Microsoft.Practices.Prism.StoreApps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public abstract class SimpleViewModel : ValidatableBindableBase
    {
        private readonly Dictionary<string, object> propertyBackingDictionary = new Dictionary<string, object>();

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            object value;
            if (propertyBackingDictionary.TryGetValue(propertyName, out value))
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
            propertyBackingDictionary[propertyName] = newValue;
            OnPropertyChanged(propertyName);
            if (Errors.IsValidationEnabled)
            {
                Errors.ValidateProperty(propertyName);
            }
            return true;
        }
    }
}
