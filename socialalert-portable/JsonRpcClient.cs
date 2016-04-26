using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bravson.Socialalert.Portable
{
    sealed class EpochDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime dateTime = (DateTime)value;
            writer.WriteValue((dateTime.Ticks - epoch.Ticks) / TimeSpan.TicksPerMillisecond);
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
        Task<T> InvokeAsync<T>(JsonRpcRequest<T> requestObject);
    }

    public sealed class JsonRpcClient : IJsonRpcClient, IDisposable
    {
        private int requestCounter;
        private readonly JsonSerializer serializer;
        private readonly HttpClient client = new HttpClient();
        private readonly Uri serverUri;

        public JsonRpcClient(String serverUrl)
        {
            serverUri = new Uri(serverUrl, UriKind.Absolute);
            serializer = new JsonSerializer();
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializer.Converters.Add(new EpochDateTimeConverter());
            serializer.Converters.Add(new StringEnumConverter());
        }

        public async Task<T> InvokeAsync<T>(JsonRpcRequest<T> requestObject)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(serverUri, requestObject.ServiceName)))
            {
                var input = new JObject();
                input["id"] = Interlocked.Increment(ref requestCounter).ToString();
                input["jsonrpc"] = "2.0";
                input["method"] = requestObject.MethodName;
                input["params"] = JObject.FromObject(requestObject, serializer);
                var inputString = input.ToString(Formatting.None);
                request.Content = new StringContent(inputString, Encoding.UTF8, "application/json-rpc");
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead))
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
                        throw new JsonRpcException((string)error["message"], (int?)error["code"]);
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
