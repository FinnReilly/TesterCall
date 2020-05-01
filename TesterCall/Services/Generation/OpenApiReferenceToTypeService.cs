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

        public OpenApiReferenceToTypeService(ILastTokenInPathService lastTokenInPathService)
        {
            _lastTokenService = lastTokenInPathService;
        }

        public Type GetType(IOpenApiObjectToTypeService objectService,
                            IObjectsProcessingKeyStore objectKeyStore,
                            OpenApiReferencedType referencedType, 
                            IDictionary<string, OpenApiObjectType> definitions)
        {
            var typeName = _lastTokenService.GetLastToken(referencedType.Type);

            if (!CurrentTypeHolder.Types.TryGetValue(typeName, out var existingType))
            {
                if (!definitions.TryGetValue(typeName, out var definedNotCreated))
                {
                    throw new NotSupportedException($"Referenced type {typeName} is not defined in OpenApi document");
                }

                //handle circular reference
                objectKeyStore.ThrowIfPresent(typeName);

                var createdType = objectService.GetType(definedNotCreated,
                                                        definitions,
                                                        typeName,
                                                        objectKeyStore);

                CurrentTypeHolder.Types[typeName] = createdType;

                return createdType;
            }

            return existingType;
        }
    }
}
