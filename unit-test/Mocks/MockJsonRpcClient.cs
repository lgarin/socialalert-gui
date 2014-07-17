using Socialalert.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialalert.Test.Mocks
{
    public class MockJsonRpcClient : IJsonRpcClient
    {
        public Func<object, object> InvokeDelegate;

        Task<T> IJsonRpcClient.InvokeAsync<T>(JsonRpcRequest<T> requestObject)
        {
            return new Task<T>(() => (T) InvokeDelegate(requestObject));
        }
    }
}
