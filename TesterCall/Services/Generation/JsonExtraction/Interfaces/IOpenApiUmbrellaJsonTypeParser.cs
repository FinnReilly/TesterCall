using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Models;

namespace TesterCall.Services.Generation.JsonExtraction.Interfaces
{
    public interface IOpenApiUmbrellaJsonTypeParser
    {
        IOpenApiType Parse(IOpenApiJsonObjectParser objectParser,
                            JsonCatchAllTypeModel model);
    }
}
