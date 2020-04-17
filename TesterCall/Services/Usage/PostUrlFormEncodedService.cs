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
        private readonly IDictionaryToUrlEncodedBodyService _contentCreateService;
        private readonly IResponseContentServiceFactory _contentReaderFactory;
        private readonly IHttpClientWrapper _client;

        public PostUrlFormEncodedService(IDictionaryToUrlEncodedBodyService dictionaryToUrlEncodedBodyService,
                                        IResponseContentServiceFactory responseContentServiceFactory,
                                        IHttpClientWrapper httpClient)
        {
            _contentCreateService = dictionaryToUrlEncodedBodyService;
            _contentReaderFactory = responseContentServiceFactory;
            _client = httpClient;
        }

        public async Task<TPostResult> GetPostResult<TPostResult>(string uri, 
                                                            IDictionary<string, string> content)
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                request.Content = _contentCreateService.GetContent(content);

                using (var response = await _client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    return await _contentReaderFactory.GetService<TPostResult>()
                                                        .ReadContent(response);
                }
            }
        }
    }
}
