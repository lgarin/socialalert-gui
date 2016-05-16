using Bravson.Socialalert.Portable.Model;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
                return await App.ServerConnection.InvokeAsync<T>(request);
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

        protected async void DisplayError(string title, Exception exception)
        {
            if (exception is JsonRpcException)
            {
                await DisplayAlert(title, (exception as JsonRpcException).ErrorCode.GetErrorMessage(Resources), "OK".Translate(Resources));
            }
            else
            {
                await DisplayAlert(title, ErrorCode.NetworkFailure.GetErrorMessage(Resources), "OK".Translate(Resources));
            }
        }
    }
}
