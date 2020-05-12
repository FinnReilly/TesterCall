using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TesterCall.Enums;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Models;
using TesterCall.Services.Generation.JsonExtraction.Models.Enums;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Generation.JsonExtraction
{
    public class OpenApiJsonEndpointsParser : IOpenApiJsonEndpointsParser
    {
        private readonly IOpenApiSpecUmbrellaTypeParser<JsonCatchAllTypeModel> _typeParser;
        private readonly IOpenApiSpecObjectParser<JsonCatchAllTypeModel> _objectParser;
        private readonly IEnumFromStringService _enumService;

        public OpenApiJsonEndpointsParser(IOpenApiSpecUmbrellaTypeParser<JsonCatchAllTypeModel> openApiUmbrellaJsonTypeParser,
                                        IOpenApiSpecObjectParser<JsonCatchAllTypeModel> openApiJsonObjectParser,
                                        IEnumFromStringService enumFromStringService)
        {
            _typeParser = openApiUmbrellaJsonTypeParser;
            _objectParser = openApiJsonObjectParser;
            _enumService = enumFromStringService;
        }

        public IEnumerable<OpenApiEndpointModel> Parse(IDictionary<string, IDictionary<string, JsonEndpointModel>> paths)
        {
            var endpoints = new List<OpenApiEndpointModel>();
            foreach (var path in paths)
            {
                foreach (var method in path.Value)
                {
                    var jsonEndpoint = method.Value;

                    var returnedEndpoint = new OpenApiEndpointModel()
                    {
                        Path = path.Key,
                        Method = _enumService.ConvertStringTo<Method>(method.Key.ToUpperInvariant()),
                        Tags = jsonEndpoint.Tags,
                        Description = jsonEndpoint.Description,
                        ShortName = jsonEndpoint.OperationId
                    };

                    var bodyParameter = jsonEndpoint.Parameters?
                                                        .FirstOrDefault(p => p.In == JsonParameterIn.body);
                    if (bodyParameter != null)
                    {
                        if (bodyParameter != null)
                        {
                            returnedEndpoint.RequestBody = new OpenApiRequestOrResponseModel()
                            {
                                Type = jsonEndpoint.Consumes != null && jsonEndpoint.Consumes.Any() ? 
                                        GetContentType(jsonEndpoint.Consumes) :
                                        "application/json",
                                Content = _typeParser.Parse(_objectParser,
                                                            bodyParameter.Schema)
                            };
                        }
                    }

                    if ((jsonEndpoint.Produces != null && jsonEndpoint.Produces.Any())
                        || jsonEndpoint.Responses.Any())
                    {
                        var successStatusResponse = jsonEndpoint.Responses
                                                                .FirstOrDefault(r => Regex.IsMatch(r.Key, "2[0-9]{2}"));

                        if (successStatusResponse.Value != null)
                        {
                            returnedEndpoint.SuccessStatusResponse = new OpenApiRequestOrResponseModel()
                            {
                                Type = GetContentType(jsonEndpoint.Produces),
                                Content = successStatusResponse.Value.Schema != null ?
                                                _typeParser.Parse(_objectParser,
                                                                    successStatusResponse.Value.Schema) :
                                                null
                            };
                        }
                    }

                    if (jsonEndpoint.Parameters != null && jsonEndpoint.Parameters.Any())
                    {
                        var parameters = new List<OpenApiParameter>();
                        foreach (var param in jsonEndpoint.Parameters)
                        {
                            if (param.In != JsonParameterIn.body)
                            {
                                parameters.Add(new OpenApiParameter()
                                {
                                    In = _enumService.ConvertStringTo<ParameterIn>(param.In.ToString().ToLower()),
                                    Schema = _typeParser.Parse(_objectParser,
                                                                param) as OpenApiPrimitiveType,
                                    Name = param.Name,
                                    Required = param.Required
                                });
                            }
                        }
                        returnedEndpoint.Parameters = parameters;
                    }

                    endpoints.Add(returnedEndpoint);
                }
            }

            return endpoints;
        }

        private string GetContentType(IEnumerable<string> supportedTypes)
        {
            var supportedContentTypes = new string[] { "application/json", "application/octet-stream" };
            foreach (var contentType in supportedTypes)
            {
                if (supportedContentTypes.Contains(contentType))
                {
                    return contentType;
                }
            }

            return supportedTypes.FirstOrDefault();
        }
    }
}
