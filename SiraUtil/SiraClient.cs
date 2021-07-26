using System;
using System.Net.Http;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace SiraUtil
{
    /// <summary>
    /// Special web client which spawns a new web client only when necessary, scoped per mod assembly.
    /// </summary>
    public class SiraClient
    {
        private bool _unique = false;
        private WebClient Main => GetClient();
        internal WebClient Client { get; set; }
        internal Assembly RootAssembly { get; set; }

        /// <summary>
        /// Sets a temporary header on the web client. It's cleared when the next request is sent.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        public void SetHeader(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("Header name cannot be null.");
            }
            _unique = true;
            if (value != null)
            {
                if (Main.client.DefaultRequestHeaders.Contains(name))
                {
                    Main.client.DefaultRequestHeaders.Remove(name);
                }
                Main.client.DefaultRequestHeaders.Add(name, value);
            }
            else
            {
                Main.client.DefaultRequestHeaders.Remove(name);
            }
        }

        /// <summary>
        /// Sets the user agent of the web client
        /// </summary>
        /// <param name="app">The name of the application (or mod in this case).</param>
        /// <param name="version">The version of the application.</param>
        public void SetUserAgent(string app, Version version)
        {
            _unique = true;
            Main.client.DefaultRequestHeaders.UserAgent.TryParseAdd($"{app}/{version}");
        }

        /// <summary>
        /// Sets the user agent of the web client
        /// </summary>
        /// <param name="app">The name of the application (or mod in this case).</param>
        /// <param name="version">The version of the application.</param>
        [Obsolete]
        public void SetUserAgent(string app, SemVer.Version version)
        {
            _unique = true;
            Main.client.DefaultRequestHeaders.UserAgent.TryParseAdd($"{app}/{version}");
        }

        /// <summary>
        /// Sets the user agent of the web client
        /// </summary>
        /// <param name="app">The name of the application (or mod in this case).</param>
        /// <param name="version">The version of the application.</param>
        public void SetUserAgent(string app, Hive.Versioning.Version version)
        {
            _unique = true;
            Main.client.DefaultRequestHeaders.UserAgent.TryParseAdd($"{app}/{version}");
        }

        private WebClient GetClient()
        {
            if (_unique)
            {
                if (!Client.clients.TryGetValue(RootAssembly, out var cli))
                {
                    cli = new WebClient(Client.config);
                    cli.Initialize();
                    Client.clients.Add(RootAssembly, cli);
                }
                return cli;
            }
            return Client;
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
            return await Main.DownloadImage(url, token, authHeader);
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
            return await Main.SendAsync(methodType, url, token, postData, authHeader, progress);
        }
    }
}