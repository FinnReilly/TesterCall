using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Services.Generation
{
    public class OpenApiUmbrellaTypeResolver : IOpenApiUmbrellaTypeResolver
    {
        private readonly IOpenApiPrimitiveToTypeService _primitiveService;
        private readonly IOpenApiReferenceToTypeService _referenceService;

        public OpenApiUmbrellaTypeResolver(IOpenApiPrimitiveToTypeService openApiPrimitiveToTypeService,
                                            IOpenApiReferenceToTypeService openApiReferenceToTypeService)
        {
            _primitiveService = openApiPrimitiveToTypeService;
            _referenceService = openApiReferenceToTypeService;
        }

        public Type GetType(IOpenApiObjectToTypeService objectService, 
                            IOpenApiType openApiType,
                            OpenApiDefinitionsModel definitions,
                            string suggestedObjectName = null)
        {
            if (Match<OpenApiPrimitiveType>(openApiType))
            {
                return _primitiveService.GetType((OpenApiPrimitiveType)openApiType);
            }

            if (Match<OpenApiReferencedType>(openApiType))
            {
                return _referenceService.GetType((OpenApiReferencedType)openApiType,
                                                definitions);
            }

            if (Match<OpenApiObjectType>(openApiType))
            {
                return objectService.GetType((OpenApiObjectType)openApiType,
                                            definitions,
                                            suggestedObjectName);
            }

            if (Match<OpenApiArrayType>(openApiType))
            {
                var arrayMemberType = GetType(objectService,
                                            openApiType,
                                            definitions,
                                            suggestedObjectName + "Member");

                return typeof(IEnumerable<>).MakeGenericType(arrayMemberType);
            }
            
            throw new NotSupportedException($"Unable to convert unsupported " +
                $"OpenApi Type {openApiType.GetType()}"); 
        }

        private bool Match<T>(object checkObject)
        {
            if (checkObject.GetType() == typeof(T))
            {
                return true;
            }

            return false;
        }
    }
}
