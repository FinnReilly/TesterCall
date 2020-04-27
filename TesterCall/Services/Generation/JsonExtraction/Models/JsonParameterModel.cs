using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Services.Generation.JsonExtraction.Models.Enums;

namespace TesterCall.Services.Generation.JsonExtraction.Models
{
    public class JsonParameterModel : JsonCatchAllTypeModel
    {
        public string Name { get; set; }
        public JsonParameterIn In { get; set; }
        public bool Required { get; set; }
        public JsonCatchAllTypeModel Schema { get; set; }
    }
}
