using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.UI.ViewModel
{
    /**
     * use regex
     * public (\w+\??) (\w+);
     * public $1 $2 { get { return Get<$1>(); } set { Set(value); } }
     * 
     * public (\w+\??)\[\] (\w+);
     * public ObservableCollection<$1> $2 = new ObservableCollection<$1>();
     */
    abstract class DictionaryViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, object> propertyBackingDictionary = new Dictionary<string, object>();
        private Dictionary<string, List<string>> validationErrors = new Dictionary<string, List<string>>();
        private readonly object threadLock = new object();

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            object value;
            if (propertyBackingDictionary.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        protected bool Set<T>(T newValue, bool broadcast = false, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            T oldValue = Get<T>(propertyName);

            if (EqualityComparer<T>.Default.Equals(newValue, oldValue)) return false;
            RaisePropertyChanging(propertyName);
            propertyBackingDictionary[propertyName] = newValue;
            RaisePropertyChanged(propertyName, oldValue, newValue, broadcast);
            ValidateProperty(newValue, propertyName);
            return true;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void OnErrorChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return validationErrors.SelectMany(err => err.Value.ToList());
            }
            List<string> errors = validationErrors[propertyName];
            if (errors == null)
            {
                return null;
            }
            return errors.ToList();
        }

        public bool HasErrors
        {
            get { return validationErrors.Any(err => err.Value != null && err.Value.Count > 0); }
        }

        public bool Valid
        {
            get { return !HasErrors; }
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (threadLock)
            {
                var validationContext = new ValidationContext(this);
                validationContext.MemberName = propertyName;
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                //clear previous errors from tested property
                if (validationErrors.ContainsKey(propertyName))
                    validationErrors.Remove(propertyName);
                OnErrorChanged(propertyName);

                HandleValidationResults(validationResults);
            }
        }

        public void Validate()
        {
            lock (threadLock)
            {
                var validationContext = new ValidationContext(this);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous errors
                var propNames = validationErrors.Keys.ToList();
                validationErrors.Clear();
                foreach (var pn in propNames)
                {
                    OnErrorChanged(pn);
                }

                HandleValidationResults(validationResults);
            }
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {

            // group error messages by property names
            var messagesGroupedByPropNames = validationResults.SelectMany(validation => validation.MemberNames, (validation, name) => new { validation, name }).GroupBy((tuple) => tuple.name, tuple => tuple.validation.ErrorMessage);

            // add errors to dictionary and inform  binding engine about errors
            foreach (var messageGroup in messagesGroupedByPropNames)
            {
                validationErrors.Add(messageGroup.Key, messageGroup.ToList());
                OnErrorChanged(messageGroup.Key);
            }

            RaisePropertyChanged(() => Valid);
        } 
    }
}
