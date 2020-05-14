using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Services.Generation.Interface
{
    public interface IOpenApiObjectToTypeService
    {
        Type GetType(OpenApiObjectType inputObject, 
                    IDictionary<string, IOpenApiType> definitions,
                    string name,
                    IObjectsProcessingKeyStore objectsProcessingKeyStore);
    }
}
