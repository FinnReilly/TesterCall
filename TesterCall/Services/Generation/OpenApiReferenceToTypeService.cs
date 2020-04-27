using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Generation
{
    public class OpenApiReferenceToTypeService : IOpenApiReferenceToTypeService
    {
        private readonly ILastTokenInPathService _lastTokenService;
        private readonly IObjectsProcessingKeyStore _objectKeyStore;
        private readonly IOpenApiObjectToTypeService _objectService;

        public OpenApiReferenceToTypeService(ILastTokenInPathService lastTokenInPathService,
                                            IObjectsProcessingKeyStore objectsProcessingKeyStore,
                                            IOpenApiObjectToTypeService openApiObjectToTypeService)
        {
            _lastTokenService = lastTokenInPathService;
            _objectKeyStore = objectsProcessingKeyStore;
            _objectService = openApiObjectToTypeService;
        }

        public Type GetType(OpenApiReferencedType referencedType, 
                            Dictionary<string, OpenApiObjectType> definitions)
        {
            var typeName = _lastTokenService.GetLastToken(referencedType.Type);

            if (!CurrentTypeHolder.Types.TryGetValue(typeName, out var existingType))
            {
                if (!definitions.TryGetValue(typeName, out var definedNotCreated))
                {
                    throw new NotSupportedException($"Referenced type {typeName} is not defined in OpenApi document");
                }

                //handle circular reference
                _objectKeyStore.ThrowIfPresent(typeName);

                var createdType = _objectService.GetType(definedNotCreated,
                                                        definitions,
                                                        typeName);

                CurrentTypeHolder.Types[typeName] = createdType;

                return createdType;
            }

            return existingType;
        }
    }
}
