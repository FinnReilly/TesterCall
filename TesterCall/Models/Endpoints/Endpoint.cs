using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Enums;

namespace TesterCall.Models.Endpoints
{
    public class Endpoint
    {
        public string Path { get; set; }
        public string ShortName { get; set; }
        public string ApiId { get; set; }
        public Method Method { get; set; }
        public IEnumerable<Parameter> PathParameters { get; set; }
        public IEnumerable<Parameter> QueryParameters { get; set; }
        public IEnumerable<Parameter> HeaderParameters { get; set; }
        public Content RequestBody { get; set; }
        public Content SuccessResponseBody { get; set; }
        public bool SuccessContentExpected { get; set; }
    }
}
