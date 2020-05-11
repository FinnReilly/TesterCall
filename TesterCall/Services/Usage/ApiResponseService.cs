using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Models;
using TesterCall.Services.Usage.Formatting.Interfaces;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Usage
{
    public class ApiResponseService : IApiResponseService
    {
        private readonly IDateTimeWrapper _dateTime;
        private readonly IHttpClientWrapper _client;
        private readonly IResponseContentServiceFactory _contentServiceFactory;

        public ApiResponseService(IDateTimeWrapper dateTimeWrapper,
                                    IHttpClientWrapper httpClientWrapper,
                                    IResponseContentServiceFactory responseContentServiceFactory)
        {
            _dateTime = dateTimeWrapper;
            _client = httpClientWrapper;
            _contentServiceFactory = responseContentServiceFactory;
        }

        public async Task<ResponseContentModel> ReturnContent(HttpRequestMessage request,
                                                                Type expectedType,
                                                                Type expectedErrorType = null)
        {
            var successContentService = _contentServiceFactory.GetService(expectedType);
            var failureContentService = _contentServiceFactory.GetService(expectedErrorType);

            var startTime = _dateTime.Now;

            using (var response = await _client.SendAsync(request))
            {
                var completeTime = _dateTime.Now;

                object content = null;
                if (response.IsSuccessStatusCode && successContentService != null)
                {
                    content = await successContentService.ReadContent(response);
                }

                if (!response.IsSuccessStatusCode && failureContentService != null)
                {
                    content = await failureContentService.ReadContent(response);
                }

                return new ResponseContentModel(completeTime - startTime,
                                                response.StatusCode,
                                                startTime,
                                                content);
            }
        }
    }
}
