using System;
using Zenject;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace SiraUtil
{
    internal class WebResponse
    {
        public readonly string ReasonPhrase;
        public readonly bool IsSuccessStatusCode;
        public readonly HttpStatusCode StatusCode;
        public readonly HttpResponseHeaders Headers;
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

		public byte[] ContentToBytes()
		{
			return _content;
		}

		public string ContentToString()
		{
			return Encoding.UTF8.GetString(_content);
		}

		public T ContentToJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(ContentToString());
        }
        public JObject ConvertToJObject()
        {
            return JObject.Parse(ContentToString());
        }
    }

    public class WebClient : IInitializable, IDisposable
    {
        private HttpClient _client;
        private readonly Config _config;

        public WebClient(Config config)
        {
            _config = config;
        }

        public void Initialize()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.UserAgent.TryParseAdd($"SiraUtil/{_config.MajorVersion}.{_config.MinorVersion}.{_config.BuildVersion}");
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }

        internal async Task<WebResponse> PostAsync(string url, object postData, CancellationToken token, AuthenticationHeaderValue authHeader = null)
        {
            return await SendAsync(HttpMethod.Post, url, token, postData, authHeader);
        }

        internal async Task<WebResponse> GetAsync(string url, CancellationToken token, AuthenticationHeaderValue authHeader = null)
        {
            return await SendAsync(HttpMethod.Get, url, token, authHeader: authHeader);
        }

        internal async Task<byte[]> DownloadImage(string url, CancellationToken token, AuthenticationHeaderValue authHeader = null)
        {
            var response = await SendAsync(HttpMethod.Get, url, token, authHeader: authHeader);

			return response.IsSuccessStatusCode ? response.ContentToBytes() : null;
		}

		internal async Task<WebResponse> SendAsync(HttpMethod methodType, string url, CancellationToken token, object postData = null, AuthenticationHeaderValue authHeader = null, IProgress<double> progress = null)
        {
            var req = new HttpRequestMessage(methodType, url);
            req.Headers.Authorization = authHeader;
            if (methodType == HttpMethod.Post && postData != null)
            {
                req.Content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
            }
            var resp = await _client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
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