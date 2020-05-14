using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiSpecModel
    {
        public OpenApiInfoModel Info { get; set; }
        public IEnumerable<OpenApiEndpointModel> Endpoints { get; set; }
        public IDictionary<string, IOpenApiType> Definitions { get; set; } 
    }
}
