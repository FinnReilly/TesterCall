using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using YamlDotNet.Serialization;

namespace TesterCall.Services.Generation.YamlExtraction.Models
{
    public class YamlSpecModel
    {
        public OpenApiInfoModel Info { get; set; }
        public IDictionary<string, YamlPathModel> Paths { get; set; }
        public YamlComponentsModel Components { get; set; }
    }
}
