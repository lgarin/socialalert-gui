using Microsoft.Practices.Prism.StoreApps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.ViewModels
{
    public abstract class SimpleViewModel : BindableBase  // ValidatableBindableBase
    {
        private readonly Dictionary<string, object> propertyBackingDictionary = new Dictionary<string, object>();

                private readonly BindableValidator _bindableValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatableBindableBase"/> class.
        /// </summary>
                public SimpleViewModel()
        {
            _bindableValidator = new BindableValidator(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is validation enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if validation is enabled for this instance; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidationEnabled
        {
            get { return this._bindableValidator.IsValidationEnabled; }
            set { this._bindableValidator.IsValidationEnabled = value; }
        }

        /// <summary>
        /// Occurs when the Errors collection changed because new errors were added or old errors were fixed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { _bindableValidator.ErrorsChanged += value; }

            remove { _bindableValidator.ErrorsChanged -= value; }
        }

        /// <summary>
        /// Gets all errors.
        /// </summary>
        /// <returns> A ReadOnlyDictionary that's key is a property name and the value is a ReadOnlyCollection of the error strings.</returns>
        public ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors()
        {
            return _bindableValidator.GetAllErrors();
        }

        /// <summary>
        /// Validates the properties of the current instance.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if all properties pass the validation rules; otherwise, false.
        /// </returns>
        public bool ValidateProperties()
        {
            return _bindableValidator.ValidateProperties();
        }

        /// <summary>
        /// Sets the error collection of this instance.
        /// </summary>
        /// <param name="entityErrors">The entity errors.</param>
        public void SetAllErrors(IDictionary<string, ReadOnlyCollection<string>> entityErrors)
        {
            _bindableValidator.SetAllErrors(entityErrors);
        }

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
            if (_bindableValidator.IsValidationEnabled)
            {
                _bindableValidator.ValidateProperty(propertyName);
            }
            return true;
        }
    }
}
