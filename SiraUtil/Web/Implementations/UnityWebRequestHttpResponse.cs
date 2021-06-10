using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace SiraUtil.Web.Implementations
{
    internal class UnityWebRequestHttpResponse : IHttpResponse
    {
        private readonly UnityWebRequest _unityWebRequest;
        public int Code => (int)_unityWebRequest.responseCode;
        public bool Successful => _unityWebRequest.isDone && !_unityWebRequest.isNetworkError && !_unityWebRequest.isHttpError;

        public UnityWebRequestHttpResponse(UnityWebRequest unityWebRequest)
        {
            _unityWebRequest = unityWebRequest;
        }

        public async Task<string?> Error()
        {
            if (Successful)
                return null;

            string body = await ReadAsStringAsync();
            if (body is null)
                return Code.ToString();

            try
            {
                return JsonConvert.DeserializeObject<ErrorBody>(body).Error;
            }
            catch
            {
                return Code.ToString();
            }
        }

        public Task<byte[]> ReadAsByteArrayAsync()
        {
            return Task.FromResult(_unityWebRequest.downloadHandler.data);
        }

        public Task<Stream> ReadAsStreamAsync()
        {
            return Task.FromResult<Stream>(new MemoryStream(_unityWebRequest.downloadHandler.data));
        }

        public Task<string> ReadAsStringAsync()
        {
            return Task.FromResult(Encoding.UTF8.GetString(_unityWebRequest.downloadHandler.data));
        }
        
        private class ErrorBody
        {
            public string Error { get; set; } = null!;
        }
    }
}
