using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Generation.JsonExtraction.Models
{
    public class JsonEndpointModel
    {
        public IEnumerable<string> Tags { get; set; }
        public string OperationId { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Consumes { get; set; }
        public IEnumerable<string> Produces { get; set; }
        public IEnumerable<JsonParameterModel> Parameters { get; set; }
        public IDictionary<string, JsonResponseModel> Responses { get; set; }
    }
}
