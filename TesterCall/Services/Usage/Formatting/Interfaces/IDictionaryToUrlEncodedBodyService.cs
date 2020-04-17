using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TesterCall.Services.Usage.Formatting.Interfaces
{
    public interface IDictionaryToUrlEncodedBodyService
    {
        StringContent GetContent(IDictionary<string, string> content);
    }
}
