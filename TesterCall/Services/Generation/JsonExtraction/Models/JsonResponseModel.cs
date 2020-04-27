using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Generation.JsonExtraction.Models
{
    public class JsonResponseModel
    {
        public string Description { get; set; }
        public JsonCatchAllTypeModel Schema { get; set; }
    }
}
