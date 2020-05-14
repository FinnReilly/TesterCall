using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Enums;
using TesterCall.Extensions;
using TesterCall.Holders;
using TesterCall.Models;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class CreateMessageService : ICreateMessageService
    {
        private readonly IUriGenerateService _uriService;
        private readonly IAddContentServiceFactory _contentServiceFactory;
        private readonly IAddHeaderParamsService _headersService;

        public CreateMessageService(IUriGenerateService uriGenerateService,
                                    IAddContentServiceFactory addContentToRequestServiceFactory,
                                    IAddHeaderParamsService addHeaderParamsService)
        {
            _uriService = uriGenerateService;
            _contentServiceFactory = addContentToRequestServiceFactory;
            _headersService = addHeaderParamsService;
        }

        public async Task<HttpRequestMessage> CreateMessage(Endpoint endpoint, 
                                                            TestEnvironment environment, 
                                                            IDictionary<string, string> queryParams, 
                                                            IDictionary<string, string> pathParams, 
                                                            IDictionary<string, string> headerParams, 
                                                            IGetAuthorisationHeaderStrategy authStrategy,
                                                            object content)
        {
            if (environment == null)
            {
                if (DefaultTestEnvironmentHolder.Environment == null)
                {
                    throw new ArgumentNullException("No test environment was supplied " +
                                                    "and there is no default " +
                                                    "test environment configured");
                }

                environment = DefaultTestEnvironmentHolder.Environment;
            }

            return await CreateMessage(environment.BaseUrl + endpoint.Path,
                                        endpoint.Method,
                                        queryParams,
                                        pathParams,
                                        headerParams,
                                        authStrategy,
                                        _contentServiceFactory.GetService(endpoint),
                                        content);
        }

        public async Task<HttpRequestMessage> CreateMessage(string url,
                                                            Method method,
                                                            IDictionary<string, string> queryParams,
                                                            IDictionary<string, string> pathParams,
                                                            IDictionary<string, string> headerParams,
                                                            IGetAuthorisationHeaderStrategy authStrategy,
                                                            IAddContentToRequestService contentService,
                                                            object content)
        {
            var returnedMessage = new HttpRequestMessage();

            returnedMessage.Method = method.ToHttpMethod();
            returnedMessage.RequestUri = _uriService.Generate(url,
                                                                pathParams,
                                                                queryParams);
            if (content != null)
            {
                contentService.AddContent(returnedMessage,
                                            content);
            }

            await _headersService.AddHeaders(returnedMessage,
                                            headerParams,
                                            authStrategy);

            return returnedMessage;
        }
    }
}
