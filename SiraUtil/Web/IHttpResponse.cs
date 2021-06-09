using System.IO;
using System.Threading.Tasks;

namespace SiraUtil.Web
{
    /// <summary>
    /// An interface which describes the stat of an HTTP response.
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        /// The HTTP status code of the response.
        /// </summary>
        int Code { get; }

        /// <summary>
        /// Whether or not the reuqest was successful or not.
        /// </summary>
        bool Successful { get; }

        /// <summary>
        /// Read the body as a stream.
        /// </summary>
        /// <returns>A stream of the response body.</returns>
        Task<Stream> ReadAsStreamAsync();

        /// <summary>
        /// Read the body as a string.
        /// </summary>
        /// <returns>The body represented as a string.</returns>
        Task<string> ReadAsStringAsync();

        /// <summary>
        /// Read the body as a byte array.
        /// </summary>
        /// <returns>The body represented as an array of bytes.</returns>
        Task<byte[]> ReadAsByteArrayAsync();

        /// <summary>
        /// The error of the response. This will first try to grab the error from an 'error' json field at the root, then go to the http response, and then will go to the native client. Will be null if the request was successful.
        /// </summary>
        Task<string?> Error();
    }
}