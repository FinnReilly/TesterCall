using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiDefinitionsModel
    {
        public Dictionary<string, IOpenApiType> Definitions { get; set; }
    }
}
