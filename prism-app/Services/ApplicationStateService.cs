using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Socialalert.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Services
{
    public interface IApplicationStateService : INotifyPropertyChanged
    {
        UserInfo CurrentUser { get; set; }
        string LastCountry { get; set; }

        bool HasUserRole(UserRole role);

        Task RestoreAsync();
        Task SaveAsync();
    }

    public class ApplicationStateService : BindableBase, IApplicationStateService
    {
        private ISessionStateService sessionStateService;

        public ApplicationStateService(ISessionStateService sessionStateService)
        {
            this.sessionStateService = sessionStateService;
        }

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            object value;
            if (sessionStateService.SessionState.TryGetValue(propertyName, out value))
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
            sessionStateService.SessionState[propertyName] = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        public UserInfo CurrentUser {
            get
            {
                return Get<UserInfo>();
            }
            set
            {
                Set<UserInfo>(value);
            }
        }

        public string LastCountry
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

        public bool HasUserRole(UserRole role)
        {
            UserInfo info = CurrentUser;
            if (info == null)
            {
                return false;
            }
            return info.Roles.Contains(role);
        }

        public Task RestoreAsync()
        {
            return sessionStateService.RestoreSessionStateAsync().ContinueWith((t) => {});
        }

        public Task SaveAsync()
        {
            return sessionStateService.SaveAsync().ContinueWith((t) => { });
        }
    }
}
