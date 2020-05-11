using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Usage.Formatting
{
    public class ReadJsonResponseContentService : IReadResponseContentService
    {
        private readonly Type _contentType;

        public ReadJsonResponseContentService(Type type)
        {
            _contentType = type;
        }

        public async Task<object> ReadContent(HttpResponseMessage response)
        {
            var jsonContent = await response.Content.ReadAsStringAsync();
            var genericMethod = typeof(JsonConvert)
                                    .GetMethods()
                                    .FirstOrDefault(m => m.IsGenericMethod 
                                                    && m.Name == "DeserializeObject")
                                    .MakeGenericMethod(_contentType);

            return genericMethod.Invoke(obj:null,
                                        new object[] { jsonContent });
        }
    }
}
