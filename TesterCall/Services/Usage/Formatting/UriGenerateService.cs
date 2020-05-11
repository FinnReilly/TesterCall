using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class UriGenerateService : IUriGenerateService
    {
        private readonly IAddPathParamsService _pathParamsService;
        private readonly IAddQueryParamsService _queryParamsService;

        public UriGenerateService(IAddPathParamsService addPathParamsService,
                                    IAddQueryParamsService addQueryParamsService)
        {
            _pathParamsService = addPathParamsService;
            _queryParamsService = addQueryParamsService;
        }

        public Uri Generate(string url,
                            IDictionary<string, string> pathParams,
                            IDictionary<string, string> queryParams)
        {
            return new Uri(_pathParamsService.UriWithPathParams(_queryParamsService.UriWithQuery(url,
                                                                                                queryParams),
                                                                pathParams));
        }
    }
}
