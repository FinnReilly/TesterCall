using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Generation.JsonExtraction.Models
{
    public class JsonCatchAllTypeModel
    {
        public string Type { get; set; }
        public string Format { get; set; }
        [JsonProperty("$ref")]
        public string Reference { get; set; }
        public IEnumerable<string> Enum { get; set; }
        public IEnumerable<JsonCatchAllTypeModel> AllOf { get; set; }
        public JsonCatchAllTypeModel Items { get; set; }
        public IDictionary<string, JsonCatchAllTypeModel> Properties { get; set; }
    }
}
