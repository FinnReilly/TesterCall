using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Extensions;
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
                            IObjectsProcessingKeyStore objectKeyStore,
                            IOpenApiType openApiType,
                            IDictionary<string, OpenApiObjectType> definitions,
                            string suggestedObjectName = null)
        {
            if (openApiType is OpenApiPrimitiveType)
            {
                return _primitiveService.GetType((OpenApiPrimitiveType)openApiType,
                                                nameIfEnum:suggestedObjectName);
            }

            if (openApiType.Matches<OpenApiReferencedType>())
            {
                return _referenceService.GetType(objectService,
                                                objectKeyStore,
                                                (OpenApiReferencedType)openApiType,
                                                definitions);
            }

            if (openApiType.Matches<OpenApiObjectType>())
            {
                return objectService.GetType((OpenApiObjectType)openApiType,
                                            definitions,
                                            suggestedObjectName,
                                            objectKeyStore);
            }

            if (openApiType.Matches<OpenApiArrayType>())
            {
                var memberType = ((OpenApiArrayType)openApiType).Items;
                var arrayMemberType = GetType(objectService,
                                            objectKeyStore,
                                            memberType,
                                            definitions,
                                            suggestedObjectName + "Member");

                return typeof(IEnumerable<>).MakeGenericType(arrayMemberType);
            }
            
            throw new NotSupportedException($"Unable to convert unsupported " +
                $"OpenApi Type {openApiType.GetType()}"); 
        }
    }
}
