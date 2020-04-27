using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiEndpointModel
    {
        public string Path { get; set; }
        public string ShortName { get; set; }
        public Method Method { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string Description { get; set; }
        public IEnumerable<OpenApiParameter> Parameters { get; set; }
        public OpenApiRequestOrResponseModel RequestBody { get; set; }
        public OpenApiRequestOrResponseModel SuccessStatusResponse { get; set; }
    }
}
