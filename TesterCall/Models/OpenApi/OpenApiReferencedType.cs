using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiReferencedType : IOpenApiType
    {
        public string Type { get; set; }
    }
}
