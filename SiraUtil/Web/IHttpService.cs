using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SiraUtil.Web
{
    /// <summary>
    /// An interface for making asynchronous HTTP calls to the internet.
    /// </summary>
    /// <remarks>
    /// This is a warning. Although this class is public, it is not meant to be inherited normally. Please ONLY receive this through Zenject and do not create your own instance.
    /// Future SiraUtil updates might add more to this interface.
    /// </remarks>
    public interface IHttpService
    {
        /// <summary>
        /// The Authroization Bearer token for your requests (primarily used for auth). Defaults to null.
        /// </summary>
        string? Token { set; }

        /// <summary>
        /// The base URL to prefix all requests with. Defaults to null.
        /// </summary>
        string? BaseURL { get;  set; }

        /// <summary>
        /// The user agent for your requests. By default, it will be set to: '[Mod Name]/[Mod Version] ([IHttpService Provider Name]; [SiraUtil Version]; Beat Saber; [Beat Saber Version])  
        /// </summary>
        string? UserAgent { get; set; }
        
        /// <summary>
        /// The default delay (in seconds) until a timeout is reached. Defaults to 60.
        /// </summary>
        int Timeout { get; set; }

        /// <summary>
        /// The default headers for your requests. Token and UserAgent are synchronized with this.
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Creates a HTTP GET request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> GetAsync(string url, IProgress<float>? progress = null, CancellationToken? cancellationToken = null);
        
        /// <summary>
        /// Creates a HTTP GET request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="timeout">The delay (in seconds) until a timeout is reached.</param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> GetAsync(string url, int timeout, IProgress<float>? progress = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Creates a HTTP POST request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="body">The content to include as a UTF-8 JSON body. The object put in here will be automatically serialized.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> PostAsync(string url, object? body = null, CancellationToken? cancellationToken = null);
        
        /// <summary>
        /// Creates a HTTP POST request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="timeout">The delay (in seconds) until a timeout is reached.</param>
        /// <param name="body">The content to include as a UTF-8 JSON body. The object put in here will be automatically serialized.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> PostAsync(string url, int timeout, object? body = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Creates a HTTP PUT request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="body">The content to include as a UTF-8 JSON body. The object put in here will be automatically serialized.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> PutAsync(string url, object? body = null, CancellationToken? cancellationToken = null);
        
        /// <summary>
        /// Creates a HTTP PUT request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="timeout">The delay (in seconds) until a timeout is reached.</param>
        /// <param name="body">The content to include as a UTF-8 JSON body. The object put in here will be automatically serialized.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> PutAsync(string url, int timeout, object? body = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Creates a HTTP PATCH request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="body">The content to include as a UTF-8 JSON body. The object put in here will be automatically serialized.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> PatchAsync(string url, object? body = null, CancellationToken? cancellationToken = null);
        
        /// <summary>
        /// Creates a HTTP PATCH request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="timeout">The delay (in seconds) until a timeout is reached.</param>
        /// <param name="body">The content to include as a UTF-8 JSON body. The object put in here will be automatically serialized.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> PatchAsync(string url, int timeout, object? body = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Creates a HTTP DELETE request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> DeleteAsync(string url, CancellationToken? cancellationToken = null);
        
        /// <summary>
        /// Creates a HTTP DELETE request.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="timeout">The delay (in seconds) until a timeout is reached.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The response.</returns>
        Task<IHttpResponse> DeleteAsync(string url, int timeout, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Sends a message asynchronously.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="body">The body of the response. It will be application/json.</param>
        /// <param name="withHeaders">Additional headers on top of the default headers. This will be combined with the default headers associated with this <see cref="IHttpService"/></param>
        /// <param name="downloadProgress">The download progress of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the token with.</param>
        /// <returns></returns>
        Task<IHttpResponse> SendAsync(HTTPMethod method, string url, string? body = null, IDictionary<string, string>? withHeaders = null, IProgress<float>? downloadProgress = null, CancellationToken? cancellationToken = null);
        
        /// <summary>
        /// Sends a message asynchronously.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="timeout">The delay (in seconds) until a timeout is reached.</param>
        /// <param name="body">The body of the response. It will be application/json.</param>
        /// <param name="withHeaders">Additional headers on top of the default headers. This will be combined with the default headers associated with this <see cref="IHttpService"/></param>
        /// <param name="downloadProgress">The download progress of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the token with.</param>
        /// <returns></returns>
        Task<IHttpResponse> SendAsync(HTTPMethod method, string url, int timeout, string? body = null, IDictionary<string, string>? withHeaders = null, IProgress<float>? downloadProgress = null, CancellationToken? cancellationToken = null);
    }
}