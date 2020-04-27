using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Services.Generation.Interface
{
    public interface IOpenApiUmbrellaTypeResolver
    {
        Type GetType(IOpenApiObjectToTypeService objectService,
                    IOpenApiType openApiType,
                    Dictionary<string, OpenApiObjectType> definitions,
                    // if class, what should name be?
                    string suggestedObjectName = null);
    }
}
