using Bravson.Socialalert.Portable.Model;
using Bravson.Socialalert.Portable.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Bravson.Socialalert.Portable
{
    sealed class EpochDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime dateTime = (DateTime)value;
            writer.WriteValue(dateTime.GetEpochMillis());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(long))
            {
                long millis = (long)reader.Value;
                return millis.FromEpochMillis();
            }
            return null;
        }
    }

    sealed class ProgressableStreamContent : HttpContent
    {
        private const int defaultBufferSize = 4096;

        private readonly Stream content;
        private readonly int bufferSize;
        private readonly IProgress<int> progress;

        public ProgressableStreamContent(Stream content, IProgress<int> progress) : this(content, defaultBufferSize, progress) { }

        public ProgressableStreamContent(Stream content, int bufferSize, IProgress<int> progress)
        {
            this.content = content;
            this.bufferSize = bufferSize;
            this.progress = progress;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var buffer = new byte[bufferSize];
            var size = (int) content.Length;
            var uploaded = 0;
            var read = 0;

            using (content)
            {
                while ((read = await content.ReadAsync(buffer, 0, buffer.Length)) > 0) 
                {
                    await stream.WriteAsync(buffer, 0, read);
                    uploaded += read;
                    progress.Report(size / uploaded);
                }
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = content.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public sealed class JsonRpcException : Exception
    {
        public JsonRpcException(string message, int? errorCode)
            : base(message)
        {
            if (errorCode.HasValue && Enum.IsDefined(typeof(ErrorCode), errorCode.Value))
            {
                ErrorCode = (ErrorCode)errorCode.Value;
            }
            else
            {
                ErrorCode = ErrorCode.Unspecified;
            }
        }

        public ErrorCode ErrorCode { get; private set; }
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

        public JsonRpcClient(Uri serverUri)
        {
            this.serverUri = serverUri;
            serializer = new JsonSerializer();
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializer.Converters.Add(new EpochDateTimeConverter());
            serializer.Converters.Add(new StringEnumConverter());
        }

        public async Task<T> InvokeAsync<T>(JsonRpcRequest<T> requestObject)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(serverUri, "rest/" + requestObject.ServiceName)))
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

        public async Task<Uri> PostAsync(Stream stream, string contentType, IProgress<int> progress)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(serverUri, "upload")))
            {
                request.Headers.Add("Content-Type", contentType);
                request.Headers.Add("Content-Length", stream.Length.ToString());
                request.Content = new ProgressableStreamContent(stream, progress);
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        return response.Headers.Location;
                    }
                    return null;
                }
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
