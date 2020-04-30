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
        private readonly IOpenApiEndpointToEndpointService _endpointService;

        public OpenApiSpecModelToGeneratedTypesService(IOpenApiObjectToTypeService openApiObjectToTypeService,
                                                        IOpenApiEndpointToEndpointService openApiEndpointToEndpointService)
        {
            _objectGenerationService = openApiObjectToTypeService;
            _endpointService = openApiEndpointToEndpointService;
        }

        public void Generate(OpenApiSpecModel inputSpec)
        {
            foreach (var definition in inputSpec.Definitions)
            {
                if (!CurrentTypeHolder.Types.TryGetValue(definition.Key, out var defined))
                {
                    CurrentTypeHolder.Types[definition.Key] = _objectGenerationService.GetType(definition.Value,
                                                                                              inputSpec.Definitions,
                                                                                              definition.Key);

                }
            }

            foreach (var endpoint in inputSpec.Endpoints)
            {
                var converted = _endpointService.GenerateEndpoint(endpoint);
                CurrentEndpointHolder.Endpoints[endpoint.ShortName] = converted;
            }
        }
    }
}
