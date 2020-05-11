using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Models;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;

namespace TesterCall.Services.Usage.Interfaces
{
    public interface IInvokeOpenApiEndpointService
    {
        Task<ResponseContentModel> InvokeEndpoint(Endpoint endpoint,
                                                    TestEnvironment testEnvironment,
                                                    Hashtable queryParams,
                                                    Hashtable pathParams,
                                                    Hashtable headerParams,
                                                    IGetAuthorisationHeaderStrategy authStrategy,
                                                    object requestBody,
                                                    bool attemptDeserializeErrorContent);
    }
}
