using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;
using Windows.Foundation;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Threading;

namespace Socialalert.Services
{
    sealed class EpochDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
          DateTime dateTime = (DateTime) value;
          writer.WriteValue(dateTime.Ticks - epoch.Ticks);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(long))
            {
                return epoch.AddTicks(TimeSpan.TicksPerMillisecond * (long)reader.Value);
            }
            return null;
        }
    }

    public sealed class JsonRpcException : Exception
    {
        public JsonRpcException(String message, int? errorCode)
            : base(message)
        {
            if (errorCode.HasValue)
            {
                HResult = errorCode.Value;
            }
        }
    }

    [JsonObject]
    public abstract class JsonRpcRequest<T>
    {
        [JsonIgnore]
        private readonly string methodName;
        [JsonIgnore]
        private readonly string serviceName;

        protected JsonRpcRequest(string serviceName, string methodName)
        {
            this.serviceName = serviceName;
            this.methodName = methodName;
        }

        [JsonIgnore]
        public String ServiceName { get { return serviceName; } }

        [JsonIgnore]
        public String MethodName { get { return methodName; } }

    }

    public interface IJsonRpcClient
    {
        Task<T> InvokeAsync<T>(Uri serverUri, JsonRpcRequest<T> requestObject);
    }

    public sealed class JsonRpcClient : IJsonRpcClient, IDisposable
    {
        private int requestCounter;
        private readonly JsonSerializer serializer;
        private readonly HttpClient client = new HttpClient();

        public JsonRpcClient()
        {
            client.DefaultRequestHeaders["contentType"] = "application/json-rpc";
            serializer = new JsonSerializer();
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializer.Converters.Add(new EpochDateTimeConverter());
        }

        public async Task<T> InvokeAsync<T>(Uri serverUri, JsonRpcRequest<T> requestObject) 
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(serverUri, requestObject.ServiceName)))
            {
                var input = new JObject();
                input["id"] = Interlocked.Increment(ref requestCounter).ToString();
                input["jsonrpc"] = "2.0";
                input["method"] = requestObject.MethodName;
                input["params"] = JObject.FromObject(requestObject, serializer);
                String inputString = input.ToString(Formatting.None);
                request.Content = new HttpStringContent(inputString, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                using (var response = await client.SendRequestAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    var resultString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                    var array = JObject.Parse(resultString);
                    var error = array["error"];
                    var result = array["result"];
                    if (error == null && result == null)
                    {
                        throw new Exception("Invalid response: " + resultString);
                    }
                    else if (error != null)
                    {
                        throw new JsonRpcException((string) error["message"], (int?) error["code"]);
                    }
                    return (T)result.ToObject(typeof(T), serializer);
                }
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}