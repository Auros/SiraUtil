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
                    if (Headers.ContainsKey("Authorization"))
                        Headers["Authorization"] = $"Bearer {value}";
                    else
                        Headers.Add("Authorization", $"Bearer {value}");
            }
        }

        public string? BaseURL { get; set; }

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

        public Task<IHttpResponse> GetAsync(string url, IProgress<float>? progress = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.GET, url, null, null, progress, cancellationToken);
        }

        public Task<IHttpResponse> PostAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.POST, url, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> PutAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.PUT, url, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> PatchAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.PATCH, url, JsonConvert.SerializeObject(body), null, null, cancellationToken);
        }

        public Task<IHttpResponse> DeleteAsync(string url, CancellationToken? cancellationToken = null)
        {
            return SendAsync(HTTPMethod.DELETE, url, null, null, null, cancellationToken);
        }

        public async Task<IHttpResponse> SendAsync(HTTPMethod method, string url, string? body = null, IDictionary<string, string>? withHeaders = null, IProgress<float>? downloadProgress = null, CancellationToken? cancellationToken = null)
        {
            // I HATE UNITY I HATE UNITY I HATE UNITY
            var response = await await UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                string newURL = url;
                if (BaseURL != null)
                    newURL = Path.Combine(BaseURL, url);
                DownloadHandler? dHandler = new DownloadHandlerBuffer();

                HTTPMethod originalMethod = method;
                if (method == HTTPMethod.POST && body != null)
                    method = HTTPMethod.PUT;

                using UnityWebRequest request = new(newURL, method.ToString(), dHandler, body == null ? null : new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)));
                request.timeout = 60;

                foreach (var header in Headers)
                    request.SetRequestHeader(header.Key, header.Value);

                if (withHeaders != null)
                    foreach (var header in withHeaders)
                        request.SetRequestHeader(header.Key, header.Value);

                if (body != null)
                {
                    request.SetRequestHeader("Content-Type", "application/json");
                }

                // some unity bull
                if (body != null && originalMethod == HTTPMethod.POST && method == HTTPMethod.PUT)
                    request.method = originalMethod.ToString();

                float _lastProgress = -1f;
                AsyncOperation asyncOp = request.SendWebRequest();
                while (!asyncOp.isDone)
                {
                    if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    {
                        request.Abort();
                        break;
                    }
                    if (downloadProgress is not null && dHandler is not null)
                    {
                        float currentProgress = asyncOp.progress;
                        if (_lastProgress != currentProgress)
                        {
                            downloadProgress.Report(currentProgress);
                            _lastProgress = currentProgress;
                        }
                    }
                    await Task.Delay(10);
                }
                downloadProgress?.Report(1f);
                bool successful = request.isDone && !request.isHttpError && !request.isNetworkError;
                return new UnityWebRequestHttpResponse(request, successful);
            });
            return response;
        }
    }
}