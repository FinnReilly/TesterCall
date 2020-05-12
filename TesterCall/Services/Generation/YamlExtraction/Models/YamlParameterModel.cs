using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Enums;

namespace TesterCall.Services.Generation.YamlExtraction.Models
{
    public class YamlParameterModel
    {
        public string Name { get; set; }
        public ParameterIn In { get; set; }
        public YamlCatchAllTypeModel Schema { get; set; }
        public bool Required { get; set; }
    }
}
