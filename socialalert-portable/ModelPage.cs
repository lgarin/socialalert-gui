using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    public abstract class ModelPage : ContentPage
    {
        private PropertyStore properties = new PropertyStore();
        private CommandStore commands = new CommandStore();

        protected ModelPage()
        {
            BindingContext = this;
        }

        protected Command Command(Action action, Func<bool> check, [CallerMemberName] string propertyName = null)
        {
            return commands.GetOrCreate(action, check, propertyName);
        }

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            return properties.GetValue<T>(propertyName);
        }

        protected bool Set<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (properties.SetValue(newValue, propertyName))
            {
                OnPropertyChanged(propertyName);
                commands.RefreshAll();
                return true;
            }
            
            return false;
        }

        public bool LoadingData
        {
            get { return Get<bool>(); }
            protected set { Set(value); }
        }

        protected async Task<T> ExecuteAsync<T>(JsonRpcRequest<T> request)
        {
            try
            {
                LoadingData = true;
                return await App.Connection.InvokeAsync<T>(request);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is JsonRpcException)
                {
                    throw ex.InnerException;
                }
                throw ex;
            }
            finally
            {
                LoadingData = false;
            }
        }

        protected virtual string getServerErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.BAD_CREDENTIALS: return "Bad credential";
                case ErrorCode.LOCKED_ACCOUNT: return "Locked account";
                default: return "Unkwnown error";
            }
        }

        protected async void DisplayError(String title, Exception exception)
        {
            if (exception is JsonRpcException)
            {
                await DisplayAlert(title, getServerErrorMessage(exception.HResult), "OK");
            }
            else
            {
                await DisplayAlert(title, "Unknown error", "OK");
            }
        }
    }
}
