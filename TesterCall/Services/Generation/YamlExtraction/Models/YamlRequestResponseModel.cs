using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Generation.YamlExtraction.Models
{
    public class YamlRequestResponseModel
    {
        public IDictionary<string, YamlContentModel> Content { get; set; }
    }
}
