using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Generation.YamlExtraction.Models
{
    public class YamlPathModel
    {
        public IEnumerable<YamlParameterModel> Parameters { get; set; }
        public YamlEndpointModel Get { get; set; }
        public YamlEndpointModel Delete { get; set; }
        public YamlEndpointModel Put { get; set; }
        public YamlEndpointModel Post { get; set; }
        public YamlEndpointModel Patch { get; set; }

    }
}
