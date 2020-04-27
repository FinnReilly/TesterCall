using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;

namespace TesterCall.Services.Generation.JsonExtraction.Models
{
    public class JsonSpecModel
    {
        public OpenApiInfoModel Info { get; set; }
        public IDictionary<string, IDictionary<string, JsonEndpointModel>> Paths { get; set; }
        public IDictionary<string, JsonCatchAllTypeModel> Definitions { get; set; }
    }
}
