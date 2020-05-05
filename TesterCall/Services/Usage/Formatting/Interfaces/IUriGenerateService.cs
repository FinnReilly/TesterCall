using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IUriGenerateService
    {
        Uri Generate(string url,
                    IDictionary<string, string> pathParams,
                    IDictionary<string, string> queryParams);
    }
}
