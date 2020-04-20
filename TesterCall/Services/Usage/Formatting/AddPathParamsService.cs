using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class AddPathParamsService : IAddPathParamsService
    {
        public string UriWithPathParams(string uri, IDictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                uri = Regex.Replace(uri, $"{{{parameter.Key}}}", $"{parameter.Value}");
            }

            return uri;
        }
    }
}
