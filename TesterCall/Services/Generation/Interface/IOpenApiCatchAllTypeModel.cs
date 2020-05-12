using System;
using System.Collections.Generic;
using System.Text;

namespace TesterCall.Services.Generation.Interface
{
    public interface IOpenApiCatchAllTypeModel<TThis>
        where TThis : IOpenApiCatchAllTypeModel<TThis>
    {
         string Type { get; set; }
         string Format { get; set; }
         string Reference { get; set; }
         IEnumerable<string> Enum { get; set; }
         IEnumerable<TThis> AllOf { get; set; }
         TThis Items { get; set; }
         IDictionary<string, TThis> Properties { get; set; }
    }
}
