using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class ReadNoContentService : IReadResponseContentService
    {
        public Task<object> ReadContent(HttpResponseMessage response)
        {
            return Task.FromResult<object>(null);
        }
    }
}
