using IPA.Utilities.Async;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SiraUtil.Web.Implementations
{
    internal class UWRHttpService : IHttpService
    {
        public IDictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        public string? Token
        {
            set
            {
                if (value is null)
                    if (Headers.ContainsKey("Authorization"))
                        Headers.Remove("Authorization");

                if (value is not null)
                    Headers["Authorization"] = $"Bearer {value}";
            }
        }

        public string? BaseURL { get; set; }

        public string? UserAgent
        {
            get => Headers.TryGetValue("User-Agent", out var value) ? value : null;
            set
            {
                if (value is null)
                    if (Headers.ContainsKey("User-Agent"))
                        Headers.Remove("User-Agent");

                if (value is not null)
                    Headers["User-Agent"] = value;
            }
        }

        public int Timeout { get; set; } = 60;

        public Task<IHttpResponse> GetAsync(string url, IProgress<float>? progress = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.GET, url, null, null, progress, cancellationToken);
        }

        public Task<IHttpResponse> GetAsync(string url, int timeout, IProgress<float>? progress = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.GET, url, timeout, null, null, progress, cancellationToken);
        }

        public Task<IHttpResponse> PostAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.POST, url, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> PostAsync(string url, int timeout, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.POST, url, timeout, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> PutAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.PUT, url, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> PutAsync(string url, int timeout, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.PUT, url, timeout, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> PatchAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.PATCH, url, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> PatchAsync(string url, int timeout, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.PATCH, url, timeout, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> DeleteAsync(string url, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.DELETE, url, null, null, null, cancellationToken);
        }

        public Task<IHttpResponse> DeleteAsync(string url, int timeout, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.DELETE, url, timeout, null, null, null, cancellationToken);
        }

        public async Task<IHttpResponse> SendAsync(HTTPMethod method, string url, string? body = null, IDictionary<string, string>? withHeaders = null, IProgress<float>? downloadProgress = null, CancellationToken? cancellationToken = null)
        {
            if (body is not null)
            {
                withHeaders ??= new Dictionary<string, string>();
                withHeaders.Add("Content-Type", "application/json");
            }
            return await SendRawAsync(method, url, body is not null ? Encoding.UTF8.GetBytes(body) : null, withHeaders, downloadProgress, cancellationToken);
        }

        public async Task<IHttpResponse> SendAsync(HTTPMethod method, string url, int timeout, string? body = null, IDictionary<string, string>? withHeaders = null, IProgress<float>? downloadProgress = null, CancellationToken? cancellationToken = null)
        {
            if (body is not null)
            {
                withHeaders ??= new Dictionary<string, string>();
                withHeaders.Add("Content-Type", "application/json");
            }
            return await SendRawAsync(method, url, body is not null ? Encoding.UTF8.GetBytes(body) : null, withHeaders, downloadProgress, cancellationToken, timeout);
        }

        public async Task<IHttpResponse> SendRawAsync(HTTPMethod method, string url, byte[]? body = null, IDictionary<string, string>? withHeaders = null, IProgress<float>? downloadProgress = null, CancellationToken? cancellationToken = null, int? timeout = null)
        {
            // I HATE UNITY I HATE UNITY I HATE UNITY
            var response = await await UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                var newURL = url;
                if (BaseURL != null)
                    newURL = Path.Combine(BaseURL, url);
                DownloadHandler? dHandler = new DownloadHandlerBuffer();

                var originalMethod = method;
                if (method == HTTPMethod.POST && body != null)
                    method = HTTPMethod.PUT;

                using UnityWebRequest request = new(newURL, method.ToString(), dHandler, body == null ? null : new UploadHandlerRaw(body));
                request.timeout = timeout ?? Timeout;

                foreach (var header in Headers)
                    request.SetRequestHeader(header.Key, header.Value);

                if (withHeaders != null)
                    foreach (var header in withHeaders)
                        request.SetRequestHeader(header.Key, header.Value);

                // some unity bull
                if (body != null && originalMethod == HTTPMethod.POST && method == HTTPMethod.PUT)
                    request.method = originalMethod.ToString();

                var lastProgress = -1f;
                AsyncOperation asyncOp = request.SendWebRequest();
                while (!asyncOp.isDone)
                {
                    if (cancellationToken is { IsCancellationRequested: true })
                    {
                        request.Abort();
                        break;
                    }
                    if (downloadProgress is not null && dHandler is not null)
                    {
                        var currentProgress = asyncOp.progress;
                        if (Math.Abs(lastProgress - currentProgress) > 0.001f)
                        {
                            downloadProgress.Report(currentProgress);
                            lastProgress = currentProgress;
                        }
                    }
                    await Task.Delay(10);
                }
                downloadProgress?.Report(1f);
                var successful = request is { isDone: true, result: UnityWebRequest.Result.Success };
                return new UnityWebRequestHttpResponse(request, successful);
            });
            return response;
        }
    }
}