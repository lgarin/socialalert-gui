using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public abstract class LoadableViewModel : ViewModel
    {
        private readonly JsonRpcClient _rpcClient;
        private readonly IAlertMessageService _alertMessageService;
        private readonly IResourceLoader _resourceLoader;
        private bool _loadingData;
        private readonly Uri serverUrl;

        protected LoadableViewModel(JsonRpcClient rpcClient, IAlertMessageService alertMessageService, IResourceLoader resourceLoader)
        {
            _rpcClient = rpcClient;
            _alertMessageService = alertMessageService;
            _resourceLoader = resourceLoader;
            serverUrl = new Uri(Application.Current.Resources["BaseServerUrl"] as string, UriKind.Absolute);
        }

        protected IResourceLoader ResourceLoader { get { return _resourceLoader;  } }

        protected IAlertMessageService AlertMessageService { get { return _alertMessageService; } }

        public bool LoadingData
        {
            get { return _loadingData; }
            private set { SetProperty(ref _loadingData, value); }
        }

        protected async Task<T> ExecuteAsync<T>(JsonRpcRequest<T> request)
        {
            T result = default(T);
            JsonRpcException rpcException = null;
            Exception exception = null;
            try
            {
                LoadingData = true;
                await Task.Delay(TimeSpan.FromSeconds((int)Application.Current.Resources["LoadDelay"]));
                result = await _rpcClient.InvokeAsync<T>(serverUrl, request);
            }
            catch (AggregateException ex)
            {
                exception = ex;
                rpcException = ex.InnerException as JsonRpcException;
            }
            catch (JsonRpcException ex)
            {
                
                rpcException = ex;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                LoadingData = false;
            }

            if (rpcException != null)
            {
                // TODO use specific error message based on error code
                var errorMessage = string.Format(CultureInfo.CurrentCulture,
                                             _resourceLoader.GetString("GeneralServiceErrorMessage"),
                                             Environment.NewLine, exception.Message);
                await _alertMessageService.ShowAsync(errorMessage, _resourceLoader.GetString("ErrorServiceUnreachable"));
                throw exception;
            }
            else if (exception != null)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture,
                                             _resourceLoader.GetString("GeneralServiceErrorMessage"),
                                             Environment.NewLine, exception.Message);
                await _alertMessageService.ShowAsync(errorMessage, _resourceLoader.GetString("ErrorServiceUnreachable"));
                throw exception;
            }
            return result;
        }
    }
}
