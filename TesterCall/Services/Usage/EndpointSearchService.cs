using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Enums;
using TesterCall.Holders;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Usage.Interfaces;

namespace TesterCall.Services.Usage
{
    public class EndpointSearchService : IEndpointSearchService
    {
        public IEnumerable<Endpoint> ByPathAndMethod(string searchString, 
                                                    Method? method,
                                                    string apiTagSearchString)
        {
            var returnValues = CurrentEndpointHolder.Endpoints.Select(e => e.Value);

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerCaseSearch = searchString.ToLowerInvariant();
                returnValues = returnValues.Where(e => e.Path
                                                        .ToLowerInvariant()
                                                        .Contains(lowerCaseSearch));
            }

            if (method.HasValue)
            {
                returnValues = returnValues.Where(e => e.Method == method.Value);
            }

            if (!string.IsNullOrEmpty(apiTagSearchString))
            {
                returnValues = SearchApiName(returnValues,
                                            apiTagSearchString);
            }

            return returnValues;
        }

        public IEnumerable<Endpoint> ByShortName(string searchString,
                                                string apiTagSearchString)
        {
            var returnList = CurrentEndpointHolder.Endpoints.Select(e => e.Value);

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerCaseSearch = searchString.ToLowerInvariant();
                returnList = returnList
                                .Where(e => e.ShortName
                                            .ToLowerInvariant()
                                            .Contains(lowerCaseSearch));
            }

            if (!string.IsNullOrEmpty(apiTagSearchString))
            {
                returnList = SearchApiName(returnList, apiTagSearchString);
            }

            return returnList;
        }

        private IEnumerable<Endpoint> SearchApiName(IEnumerable<Endpoint> endpoints,
                                                    string apiName)
        {
            var lowerCase = apiName.ToLowerInvariant();

            return endpoints.Where(e => e.ApiId
                                        .ToLowerInvariant()
                                        .Contains(lowerCase));
        }
    }
}
