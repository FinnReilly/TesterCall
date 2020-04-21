using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiPrimitiveType : IOpenApiType
    {
        public virtual string Type { get; set; }
        public string Format { get; set; }
        public bool ReadOnly { get; set; }
    }
}
