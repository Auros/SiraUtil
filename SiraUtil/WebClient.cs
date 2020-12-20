using System;
using Zenject;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace SiraUtil
{
    /// <summary>
    /// A custom web response class from the <see cref="WebClient"/>
    /// </summary>
    public class WebResponse
    {
        /// <summary>
        /// The reason phrase of the response.
        /// </summary>
        public readonly string ReasonPhrase;

        /// <summary>
        /// Whether or not the request was successful.
        /// </summary>
        public readonly bool IsSuccessStatusCode;

        /// <summary>
        /// The HTTP Status code of the response.
        /// </summary>
        public readonly HttpStatusCode StatusCode;

        /// <summary>
        /// The headers of the response.
        /// </summary>
        public readonly HttpResponseHeaders Headers;

        /// <summary>
        /// The request message.
        /// </summary>
        public readonly HttpRequestMessage RequestMessage;

        private readonly byte[] _content;

        internal WebResponse(HttpResponseMessage resp, byte[] content)
        {
            _content = content;
            Headers = resp.Headers;
            StatusCode = resp.StatusCode;
            ReasonPhrase = resp.ReasonPhrase;
            RequestMessage = resp.RequestMessage;
            IsSuccessStatusCode = resp.IsSuccessStatusCode;
        }

        /// <summary>
        /// Converts the response to a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ContentToBytes()
        {
            return _content;
        }

        /// <summary>
        /// Converts the response to a string.
        /// </summary>
        /// <returns></returns>
        public string ContentToString()
        {
            return Encoding.UTF8.GetString(_content);
        }

        /// <summary>
        /// Deserialize the content to a typed object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize the content into.</typeparam>
        /// <returns>The deserialized object.</returns>
        public T ContentToJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(ContentToString());
        }

        /// <summary>
        /// Deserializes the content into a <see cref="JObject"/>
        /// </summary>
        /// <returns>The deserialized object as a <see cref="JObject"/></returns>
        public JObject ConvertToJObject()
        {
            return JObject.Parse(ContentToString());
        }
    }

    /// <summary>
    /// A wrapper around the <seealso cref="HttpClient"/> which makes it easier to do simple web requests. You can receive this in any Container. Based off of nate1280's WebClient.
    /// </summary>
    public class WebClient : IInitializable, IDisposable
    {
        internal HttpClient client;
        internal readonly Config config;
        internal readonly Dictionary<Assembly, WebClient> clients = new Dictionary<Assembly, WebClient>();

        internal WebClient(Config config)
        {
            this.config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.TryParseAdd($"SiraUtil/{config.Version}");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
            }
            foreach (var cli in clients)
            {
                cli.Value.Dispose();
            }
        }

        /// <summary>
        /// Asynchronously send a POST request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="postData">The body data for the request.</param>
        /// <param name="token">The cancellation token for the request.</param>
        /// <param name="authHeader">The header for the request.</param>
        /// <returns></returns>
        public async Task<WebResponse> PostAsync(string url, object postData, CancellationToken token, AuthenticationHeaderValue authHeader = null)
        {
            return await SendAsync(HttpMethod.Post, url, token, postData, authHeader);
        }

        /// <summary>
        /// Asynchronously send a GET request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="token">The cancellation token for the request.</param>
        /// <param name="authHeader">The header for the request.</param>
        /// <returns></returns>
        public async Task<WebResponse> GetAsync(string url, CancellationToken token, AuthenticationHeaderValue authHeader = null)
        {
            return await SendAsync(HttpMethod.Get, url, token, authHeader: authHeader);
        }

        /// <summary>
        /// Downloads an image at a URL and returns its raw byte data.
        /// </summary>
        /// <param name="url">The URL where the image is located.</param>
        /// <param name="token">The cancellation token for the request.</param>
        /// <param name="authHeader">The header for the request.</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadImage(string url, CancellationToken token, AuthenticationHeaderValue authHeader = null)
        {
            var response = await SendAsync(HttpMethod.Get, url, token, authHeader: authHeader);
            return response.IsSuccessStatusCode ? response.ContentToBytes() : null;
        }

        /// <summary>
        /// Asynchronously send a web request.
        /// </summary>
        /// <param name="methodType">The HTTP request type of the request.</param>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="token">The cancellation token of the request.</param>
        /// <param name="postData">The body of the request.</param>
        /// <param name="authHeader">The header of the request.</param>
        /// <param name="progress">The progress reporter of the request.</param>
        /// <returns></returns>
        public async Task<WebResponse> SendAsync(HttpMethod methodType, string url, CancellationToken token, object postData = null, AuthenticationHeaderValue authHeader = null, IProgress<double> progress = null)
        {
            var req = new HttpRequestMessage(methodType, url);
            req.Headers.Authorization = authHeader;
            if (methodType == HttpMethod.Post && postData != null)
            {
                req.Content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
            }
            var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            using (var memoryStream = new MemoryStream())
            using (var stream = await resp.Content.ReadAsStreamAsync())
            {
                var buffer = new byte[8192];
                var bytesRead = 0;
                long? contentLength = resp.Content.Headers.ContentLength;
                var totalRead = 0;
                progress?.Report(0);
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }
                    if (contentLength != null)
                    {
                        progress?.Report(totalRead / (double)contentLength);
                    }

                    await memoryStream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;
                }
                progress?.Report(1);
                byte[] bytes = memoryStream.ToArray();
                return new WebResponse(resp, bytes);
            }
        }
    }
}