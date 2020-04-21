using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiObjectType : IOpenApiType
    {
        public string Type { get; set; } = "object";
        public Dictionary<string, IOpenApiType> Properties { get; set; }
        public IEnumerable<IOpenApiType> AllOf { get; set; }

        public OpenApiDefinitionsModel Definitions { get; set; }
    }
}
