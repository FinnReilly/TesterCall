using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IAddHeaderParamsService
    {
        Task AddHeaders(HttpRequestMessage request,
                        IDictionary<string, string> headers,
                        IGetAuthorisationHeaderStrategy authStrategy);
    }
}
