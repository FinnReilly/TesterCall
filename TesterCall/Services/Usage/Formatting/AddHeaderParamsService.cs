using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class AddHeaderParamsService : IAddHeaderParamsService
    {
        public async Task AddHeaders(HttpRequestMessage request, 
                                    IDictionary<string, string> headers, 
                                    IGetAuthorisationHeaderStrategy authStrategy)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (authStrategy != null)
            {
                request.Headers.Add(HttpRequestHeader.Authorization.ToString(),
                                    new List<string>{
                                        await authStrategy.GetHeader()
                                    });
            }
        }
    }
}
