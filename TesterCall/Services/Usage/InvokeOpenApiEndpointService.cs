using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Extensions;
using TesterCall.Models;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage.AuthStrategies.Interfaces;
using TesterCall.Services.Usage.Formatting.Interfaces;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Usage
{
    public class InvokeOpenApiEndpointService : IInvokeOpenApiEndpointService
    {
        private readonly IApiResponseService _apiResponseService;
        private readonly ICheckRequiredParametersService _parameterCheckService;
        private readonly ICreateMessageService _createMessageService;
        private readonly IResponseRecorderService _responseRecorder;

        public InvokeOpenApiEndpointService(IApiResponseService apiResponseService,
                                            ICheckRequiredParametersService checkRequiredParametersService,
                                            ICreateMessageService createMessageService,
                                            IResponseRecorderService responseRecorderService)
        {
            _apiResponseService = apiResponseService;
            _parameterCheckService = checkRequiredParametersService;
            _createMessageService = createMessageService;
            _responseRecorder = responseRecorderService;
        }

        public async Task<ResponseContentModel> InvokeEndpoint(Endpoint endpoint, 
                                                                TestEnvironment testEnvironment, 
                                                                Hashtable queryParams, 
                                                                Hashtable pathParams, 
                                                                Hashtable headerParams,
                                                                IGetAuthorisationHeaderStrategy authStrategy,
                                                                object requestBody,
                                                                bool attemptDeserializeErrorContent = false)
        {
            var queryDict = queryParams.AsStringStringDictionary();
            var pathDict = pathParams.AsStringStringDictionary();
            var headerDict = headerParams.AsStringStringDictionary();

            _parameterCheckService.CheckRequiredParametersPresent(endpoint,
                                                                    queryDict,
                                                                    pathDict,
                                                                    headerDict);

            using (var request = await _createMessageService.CreateMessage(endpoint,
                                                                            testEnvironment,
                                                                            queryDict,
                                                                            pathDict,
                                                                            headerDict,
                                                                            authStrategy,
                                                                            requestBody))
            {
                var responseModel = await _apiResponseService.ReturnContent(request,
                                                                            endpoint.SuccessResponseBody?.Type,
                                                                            attemptDeserializeErrorContent ?
                                                                                typeof(object) :
                                                                                null);

                _responseRecorder.RecordIfRequired(responseModel);

                return responseModel;
            }
        }
    }
}
