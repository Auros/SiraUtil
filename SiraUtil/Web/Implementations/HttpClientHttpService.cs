using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SiraUtil.Web.Implementations
{
    internal class HttpClientHttpService : IHttpService, IDisposable
    {
        private readonly HttpClient _httpClient;
        public IDictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        public string? Token
        {
            set
            {
                if (value is null)
                    if (Headers.ContainsKey("Authorization"))
                        Headers.Remove("Authorization");

                if (value is not null)
                    if (Headers.ContainsKey("Authorization"))
                        Headers["Authorization"] = $"Bearer {value}";
                    else
                        Headers.Add("Authorization", $"Bearer {value}");
            }
        }

        public string? BaseURL
        {
            get => _httpClient.BaseAddress.ToString();
            set => _httpClient.BaseAddress = new Uri(value);
        }

        public string? UserAgent
        {
            get
            {
                if (Headers.TryGetValue("User-Agent", out string value))
                    return value;
                else return null;
            }
            set
            {
                if (value is null)
                    if (Headers.ContainsKey("User-Agent"))
                        Headers.Remove("User-Agent");

                if (value is not null)
                    if (Headers.ContainsKey("User-Agent"))
                        Headers["User-Agent"] = value;
                    else
                        Headers.Add("User-Agent", value);
            }
        }

        public HttpClientHttpService()
        {
            _httpClient = new HttpClient();
        }

        public Task<IHttpResponse> GetAsync(string url, IProgress<float>? progress = null, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpResponse> PostAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpResponse> PutAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpResponse> PatchAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpResponse> DeleteAsync(string url, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<IHttpResponse> SendAsync(HTTPMethod method, string url, string? body = null, IDictionary<string, string>? withHeaders = null, IProgress<float>? downloadProgress = null, CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}