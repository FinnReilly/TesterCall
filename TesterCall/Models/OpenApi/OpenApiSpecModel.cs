using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiSpecModel
    {
        public OpenApiInfoModel Info { get; set; }
        public IEnumerable<OpenApiEndpointModel> Endpoints { get; set; }
        public IDictionary<string, OpenApiObjectType> Definitions { get; set; } 
    }
}
