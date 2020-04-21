using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiEnumType : OpenApiPrimitiveType, IOpenApiType
    {
        public override string Type { get; set; } = "string";
        public string[] Enum { get; set; }
        public string GeneratedTypeName { get; set; }
    }
}
