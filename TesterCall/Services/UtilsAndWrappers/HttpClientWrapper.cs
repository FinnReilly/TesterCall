using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _client;

        public HttpClientWrapper()
        {
            _client = new HttpClient();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            return _client.SendAsync(requestMessage);
        }
    }
}
