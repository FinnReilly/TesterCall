using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.Formatting.Interfaces;
using TesterCall.Services.Usage.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Usage
{
    public class PostUrlFormEncodedService : IPostUrlFormEncodedService
    {
        private readonly IResponseContentServiceFactory _contentReaderFactory;
        private readonly IHttpClientWrapper _client;
        private readonly IDateTimeWrapper _dateTime;

        public PostUrlFormEncodedService(IResponseContentServiceFactory responseContentServiceFactory,
                                        IHttpClientWrapper httpClient,
                                        IDateTimeWrapper dateTimeWrapper)
        {
            _contentReaderFactory = responseContentServiceFactory;
            _client = httpClient;
            _dateTime = dateTimeWrapper;
        }

        public async Task<(TimeSpan responseTime, TPostResult response)> GetPostResult<TPostResult>(string uri, 
                                                                        IDictionary<string, string> content)
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new FormUrlEncodedContent(content);

                var startTime = _dateTime.Now;
                using (var response = await _client.SendAsync(request))
                {
                    var endTime = _dateTime.Now;
                    response.EnsureSuccessStatusCode();
                    var result = await _contentReaderFactory.GetService(typeof(TPostResult))
                                                            .ReadContent(response);

                    return (endTime - startTime, (TPostResult)result);
                }
            }
        }
    }
}
