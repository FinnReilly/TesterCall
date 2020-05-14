using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Services.Generation.Interface
{
    public interface IOpenApiReferenceToTypeService
    {
        Type GetType(IOpenApiObjectToTypeService openApiObjectToTypeService,
                    IOpenApiUmbrellaTypeResolver typeResolver,
                    IObjectsProcessingKeyStore objectsProcessingKeyStore,
                    OpenApiReferencedType referencedType, 
                    IDictionary<string, IOpenApiType> definitions);
    }
}
