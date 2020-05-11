using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TesterCall.Services.UtilsAndWrappers.Interfaces
{
    public interface IHttpClientWrapper : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage);
    }
}
