using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Generation.Interface;
using YamlDotNet.Serialization;

namespace TesterCall.Services.Generation.YamlExtraction.Models
{
    public class YamlCatchAllTypeModel : IOpenApiCatchAllTypeModel<YamlCatchAllTypeModel>
    {
            public string Type { get; set; }
            public string Format { get; set; }
            [YamlMember(Alias = "$ref")]
            public string Reference { get; set; }
            public IEnumerable<string> Enum { get; set; }
            public IEnumerable<YamlCatchAllTypeModel> AllOf { get; set; }
            public YamlCatchAllTypeModel Items { get; set; }
            public IDictionary<string, YamlCatchAllTypeModel> Properties { get; set; }
    }
}
