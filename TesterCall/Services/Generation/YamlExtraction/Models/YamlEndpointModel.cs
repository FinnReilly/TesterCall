using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Generation.YamlExtraction.Models
{
    public class YamlEndpointModel
    {
        public IEnumerable<string> Tags { get; set; }
        public string Description { get; set; }
        public IEnumerable<YamlParameterModel> Parameters { get; set; }
        public YamlRequestResponseModel RequestBody { get; set; }
        public IDictionary<string, YamlRequestResponseModel> Responses { get; set; }
    }
}
