using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bravson.Socialalert.Portable
{
    abstract class LoadableModel : SimpleModel
    {
        private Page page;

        protected LoadableModel(Page page)
        {
            this.page = page;
        }

        [JsonIgnore]
        public bool LoadingData
        {
            get { return Get<bool>(); }
            protected set { Set(value); }
        }

        protected async Task<T> ExecuteAsync<T>(JsonRpcRequest<T> request)
        {
            T result = default(T);
            JsonRpcException rpcException = null;
            Exception exception = null;
            try
            {
                LoadingData = true;
                result = await App.Current.RequestServerAsync<T>(request);
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
                throw rpcException;
            }
            else if (exception != null)
            {
                
                var errorMessage = string.Format(CultureInfo.CurrentCulture,
                                             "The following error messages were received from the service: {0} {1}",
                                             Environment.NewLine, exception.Message);
                await page.DisplayAlert("Service is unreachable.", errorMessage, "OK");
                throw exception;
            }
            return result;
        }
    }
}
