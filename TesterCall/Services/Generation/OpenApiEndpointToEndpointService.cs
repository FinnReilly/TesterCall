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
                PathParameters = SelectParameterByLocation(inputModel, ParameterIn.path),
                QueryParameters = SelectParameterByLocation(inputModel, ParameterIn.query),
                HeaderParameters = SelectParameterByLocation(inputModel, ParameterIn.header),
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

        private IEnumerable<Parameter> SelectParameterByLocation(OpenApiEndpointModel parentEndpoint,
                                                                ParameterIn paramIn)
        {
            return parentEndpoint.Parameters?
                                .Where(p => p.In == paramIn)
                                .Select(p => new Parameter()
                                {
                                    Name = p.Name,
                                    Required = p.Required,
                                    Type = _primitiveService.GetType(p.Schema,
                                                    parentEndpoint.ShortName + p.Name)
                                });
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
