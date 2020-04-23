using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Models.OpenApi.Interfaces
{
    public class OpenApiRequestOrResponseModel
    {
        public string Type { get; set; }
        public IOpenApiType Content { get; set; }
    }
}
