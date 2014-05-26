using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Socialalert.ViewModels
{
    public abstract class LoadableViewModel : ViewModel
    {
        private Uri serverUrl;
        private int loadDelay;
        private bool loadingData;

        public LoadableViewModel()
        {
            DumpData = new DelegateCommand(WriteJson);
        }

        [InjectionMethod]
        public void Init(ResourceDictionary resourceDictionary)
        {
            serverUrl = new Uri(resourceDictionary["BaseServerUrl"] as string, UriKind.Absolute);
            loadDelay = (int) resourceDictionary["LoadDelay"];
        }

        [Dependency]
        protected IJsonRpcClient JsonRpcClient { get; set; }

        [Dependency]
        protected IResourceLoader ResourceLoader { get; set; }

        [Dependency]
        protected IAlertMessageService AlertMessageService { get; set; }

        [Dependency]
        protected INavigationService NavigationService { get; set; }

        [Dependency]
        protected ResourceDictionary ResourceDictionary { get; set; }

        public DelegateCommand DumpData { get; private set; }

        private void WriteJson()
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            serializer.ContractResolver = new ViewModelContractResolver(GetType().Namespace);
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, this);
                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetText(writer.ToString());
                Clipboard.SetContent(dataPackage);
            }
        }

        public bool LoadingData
        {
            get { return loadingData; }
            private set { SetProperty(ref loadingData, value); }
        }

        protected async Task<T> ExecuteAsync<T>(JsonRpcRequest<T> request)
        {
            T result = default(T);
            JsonRpcException rpcException = null;
            Exception exception = null;
            try
            {
                LoadingData = true;
                if (loadDelay > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(loadDelay));
                }
                result = await JsonRpcClient.InvokeAsync<T>(serverUrl, request);
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
                                             ResourceLoader.GetString("GeneralServiceErrorMessage"),
                                             Environment.NewLine, exception.Message);
                await AlertMessageService.ShowAsync(errorMessage, ResourceLoader.GetString("ErrorServiceUnreachable"));
                throw exception;
            }
            else if (exception != null)
            {
                var errorMessage = string.Format(CultureInfo.CurrentCulture,
                                             ResourceLoader.GetString("GeneralServiceErrorMessage"),
                                             Environment.NewLine, exception.Message);
                await AlertMessageService.ShowAsync(errorMessage, ResourceLoader.GetString("ErrorServiceUnreachable"));
                throw exception;
            }
            return result;
        }
    }
}
