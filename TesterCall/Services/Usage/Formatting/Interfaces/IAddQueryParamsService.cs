using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IAddQueryParamsService
    {
        string UriWithQuery(string uri, IDictionary<string, string> parameters);
    }
}
