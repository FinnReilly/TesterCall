using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Enums;
using TesterCall.Models;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface ICreateMessageService
    {
        Task<HttpRequestMessage> CreateMessage(Endpoint endpoint,
                                                TestEnvironment environment,
                                                IDictionary<string, string> queryParams,
                                                IDictionary<string, string> pathParams,
                                                IDictionary<string, string> headerParams,
                                                IGetAuthorisationHeaderStrategy authStrategy,
                                                object content);

        Task<HttpRequestMessage> CreateMessage(string url,
                                                Method method,
                                                IDictionary<string, string> queryParams,
                                                IDictionary<string, string> pathParams,
                                                IDictionary<string, string> headerParams,
                                                IGetAuthorisationHeaderStrategy authStrategy,
                                                IAddContentToRequestService contentService,
                                                object content);
    }
}
