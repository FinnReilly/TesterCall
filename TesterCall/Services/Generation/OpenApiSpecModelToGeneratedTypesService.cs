using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Services.Generation
{
    public class OpenApiSpecModelToGeneratedTypesService : IOpenApiSpecModelToGeneratedTypesService
    {
        private readonly IOpenApiObjectToTypeService _objectGenerationService;
        private readonly IOpenApiUmbrellaTypeResolver _typeResolver;
        private readonly IOpenApiEndpointToEndpointService _endpointService;

        public OpenApiSpecModelToGeneratedTypesService(IOpenApiObjectToTypeService openApiObjectToTypeService,
                                                        IOpenApiUmbrellaTypeResolver openApiUmbrellaTypeResolver,
                                                        IOpenApiEndpointToEndpointService openApiEndpointToEndpointService)
        {
            _objectGenerationService = openApiObjectToTypeService;
            _typeResolver = openApiUmbrellaTypeResolver;
            _endpointService = openApiEndpointToEndpointService;
        }

        public void Generate(OpenApiSpecModel inputSpec)
        {
            //TODO
            //need better way to do this - factory?
            var keyStore = new ObjectsProcessingKeyStore();

            foreach (var definition in inputSpec.Definitions)
            {
                if (!CurrentTypeHolder.Types.TryGetValue(definition.Key, out var defined))
                {
                    CurrentTypeHolder.Types[definition.Key] = _typeResolver.GetType(_objectGenerationService,
                                                                                    keyStore,
                                                                                    definition.Value,
                                                                                    inputSpec.Definitions,
                                                                                    definition.Key);

                }
            }

            foreach (var endpoint in inputSpec.Endpoints)
            {
                var converted = _endpointService.GenerateEndpoint(endpoint,
                                                                    keyStore);
                converted.ApiId = inputSpec.Info.Title;
                CurrentEndpointHolder.Endpoints[endpoint.ShortName] = converted;
            }
        }
    }
}
