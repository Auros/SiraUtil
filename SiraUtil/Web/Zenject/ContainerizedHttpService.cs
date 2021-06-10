using IPA.Loader;
using IPA.Utilities;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SiraUtil.Web.Zenject
{
    internal class ContainerizedHttpService : IHttpService
    {
        private IHttpService _childService = null!;

        public string? Token
        {
            set => _childService.Token = value;
        }

        public string? BaseURL
        {
            get => _childService.BaseURL;
            set => _childService.BaseURL = value;
        }

        public string? UserAgent
        {
            get => _childService.UserAgent;
            set => _childService.UserAgent = value;
        }

        public IDictionary<string, string> Headers => _childService.Headers;

        public void Setup(IHttpService childService)
        {
            _childService = childService;
        }

        public Task<IHttpResponse> DeleteAsync(string url, CancellationToken? cancellationToken = null)
        {
            return _childService.DeleteAsync(url, cancellationToken);
        }

        public Task<IHttpResponse> GetAsync(string url, IProgress<float>? progress = null, CancellationToken? cancellationToken = null)
        {
            return _childService.GetAsync(url, progress, cancellationToken);
        }

        public Task<IHttpResponse> PatchAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return _childService.PatchAsync(url, body, cancellationToken);
        }

        public Task<IHttpResponse> PostAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return _childService.PostAsync(url, body, cancellationToken);
        }

        public Task<IHttpResponse> PutAsync(string url, object? body = null, CancellationToken? cancellationToken = null)
        {
            return _childService.PutAsync(url, body, cancellationToken);
        }

        public Task<IHttpResponse> SendAsync(HTTPMethod method, string url, string? body = null, IDictionary<string, string>? withHeaders = null, IProgress<float>? downloadProgress = null, CancellationToken? cancellationToken = null)
        {
            return _childService.SendAsync(method, url, body, withHeaders, downloadProgress, cancellationToken);
        }
    }
}
