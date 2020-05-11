using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class AddQueryParamsService : IAddQueryParamsService
    {
        public string UriWithQuery(string uri, IDictionary<string, string> parameters)
        {
            var queryParamStrings = new List<string>();

            string queryString = null;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    queryParamStrings.Add($"{parameter.Key}={parameter.Value}");
                }

                queryString = string.Join("&", queryParamStrings);
            }

            if (!string.IsNullOrEmpty(queryString))
            {
                return $"{uri}?{queryString}";
            }

            return uri;
        }
    }
}
