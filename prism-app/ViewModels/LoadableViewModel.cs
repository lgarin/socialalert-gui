using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        private int loadDelay;
        private bool loadingData;

        [InjectionMethod]
        public void Init(ResourceDictionary resourceDictionary)
        {
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

        protected void OnPropertyChanged(Expression<Func<object>> expression)
        {
            OnPropertyChanged(ExtractMemberName(expression));
        }

        protected static string ExtractMemberName(Expression<Func<object>> expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Value must be a lamda expression", "expression");
            }

            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (expression == null)
            {
                throw new ArgumentException("Value should be a member expression", "expression");
            }
            return memberExpression.Member.Name;
        }

        public string SerializeToJson()
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            serializer.ContractResolver = new ViewModelContractResolver(GetType().Namespace);
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, this);
                return writer.ToString();
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
                result = await JsonRpcClient.InvokeAsync<T>(request);
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
                                             ResourceLoader.GetString("GeneralServiceErrorMessage"),
                                             Environment.NewLine, exception.Message);
                await AlertMessageService.ShowAsync(errorMessage, ResourceLoader.GetString("ErrorServiceUnreachable"));
                throw exception;
            }
            return result;
        }
    }
}
