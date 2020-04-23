using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;

namespace TesterCall.Services.Generation.Interface
{
    public interface IOpenApiObjectToTypeService
    {
        Type GetType(OpenApiObjectType inputObject, 
                    OpenApiDefinitionsModel definitions,
                    string name);
    }
}
