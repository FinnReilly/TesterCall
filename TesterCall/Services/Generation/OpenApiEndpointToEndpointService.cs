using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Enums;
using TesterCall.Models.Endpoints;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Services.Generation
{
    public class OpenApiEndpointToEndpointService : IOpenApiEndpointToEndpointService
    {
        private readonly IOpenApiUmbrellaTypeResolver _typeResolver;
        private readonly IOpenApiPrimitiveToTypeService _primitiveService;
        private readonly IOpenApiObjectToTypeService _objectToTypeService;

        public OpenApiEndpointToEndpointService(IOpenApiUmbrellaTypeResolver openApiUmbrellaTypeResolver,
                                                IOpenApiPrimitiveToTypeService openApiPrimitiveToTypeService,
                                                IOpenApiObjectToTypeService openApiObjectToTypeService)
        {
            _typeResolver = openApiUmbrellaTypeResolver;
            _primitiveService = openApiPrimitiveToTypeService;
            _objectToTypeService = openApiObjectToTypeService;
        }

        public Endpoint GenerateEndpoint(OpenApiEndpointModel inputModel,
                                        IObjectsProcessingKeyStore keyStore)
        {
            var output = new Endpoint()
            {
                Path = inputModel.Path,
                ShortName = inputModel.ShortName,
                Method = inputModel.Method,
                PathParameters = inputModel.Parameters?
                                            .Where(p => p.In == ParameterIn.path)
                                            .Select(p => ConvertParameter(p, inputModel)),
                QueryParameters = inputModel.Parameters?
                                            .Where(p => p.In == ParameterIn.query)
                                            .Select(p => ConvertParameter(p, inputModel)),
                RequestBody = inputModel.RequestBody == null ?
                                            null :
                                            ConvertContent(inputModel.RequestBody,
                                                            $"{inputModel.ShortName}Request",
                                                            keyStore),
                SuccessResponseBody = inputModel.SuccessStatusResponse == null ?
                                                    null :
                                                    ConvertContent(inputModel.SuccessStatusResponse,
                                                                    $"{inputModel.ShortName}Result",
                                                                    keyStore)
            };

            output.SuccessContentExpected = output.SuccessResponseBody != null;
            return output;
        }

        private Parameter ConvertParameter(OpenApiParameter input,
                                            OpenApiEndpointModel parentEndpoint)
        {
            return new Parameter()
            {
                Name = input.Name,
                Required = input.Required,
                Type = _primitiveService.GetType(input.Schema,
                                                parentEndpoint.ShortName + input.Name)
            };
        }

        private Content ConvertContent(OpenApiRequestOrResponseModel input,
                                        string name,
                                        IObjectsProcessingKeyStore keyStore)
        {
            return new Content()
            {
                ContentType = input.Type,
                Type = input.Content == null ?
                            null :
                            _typeResolver.GetType(_objectToTypeService,
                                                    keyStore,
                                                    input.Content,
                                                    null,
                                                    name)
            };
        }
    }
}
