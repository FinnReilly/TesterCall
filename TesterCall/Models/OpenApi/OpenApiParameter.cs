using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Enums;

namespace TesterCall.Models.OpenApi
{
    public class OpenApiParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public OpenApiPrimitiveType Schema { get; set; }
        public ParameterIn In { get; set; }
        public bool Required { get; set; }
    }
}
