using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class ReadJsonResponseContentService<TContent> : IReadReponseContentService<TContent>
    {
        public async Task<TContent> ReadContent(HttpResponseMessage response)
        {
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TContent>(jsonContent);
        }
    }
}
