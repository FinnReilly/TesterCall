using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TesterCall.Enums;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.YamlExtraction.Interfaces;
using TesterCall.Services.Generation.YamlExtraction.Models;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Generation.YamlExtraction
{
    public class OpenApiYamlEndpointsParser : IOpenApiYamlEndpointsParser
    {
        private readonly IOpenApiSpecUmbrellaTypeParser<YamlCatchAllTypeModel> _typeParser;
        private readonly IOpenApiSpecObjectParser<YamlCatchAllTypeModel> _objectParser;
        private readonly IEnumFromStringService _enumService;

        public OpenApiYamlEndpointsParser(IOpenApiSpecUmbrellaTypeParser<YamlCatchAllTypeModel> openApiSpecUmbrellaTypeParser,
                                            IOpenApiSpecObjectParser<YamlCatchAllTypeModel> openApiSpecObjectParser,
                                            IEnumFromStringService enumFromStringService)
        {
            _typeParser = openApiSpecUmbrellaTypeParser;
            _objectParser = openApiSpecObjectParser;
            _enumService = enumFromStringService;
        }

        public IEnumerable<OpenApiEndpointModel> Parse(IDictionary<string, 
                                                                    YamlPathModel> paths)
        {
            var endpoints = new List<OpenApiEndpointModel>();
            foreach (var path in paths)
            {
                var pathScopeParams = path.Value.Parameters != null && path.Value.Parameters.Any() ?
                                            ParseParameters(path.Value.Parameters) :
                                            null;

                if (path.Value.Delete != null)
                {
                    endpoints.Add(ParseIndividualEndpoint(path.Value.Delete,
                                                            pathScopeParams,
                                                            "DELETE",
                                                            path.Key));
                }

                if (path.Value.Get != null)
                {
                    endpoints.Add(ParseIndividualEndpoint(path.Value.Get,
                                                            pathScopeParams,
                                                            "GET",
                                                            path.Key));
                }

                if (path.Value.Patch != null)
                {
                    endpoints.Add(ParseIndividualEndpoint(path.Value.Patch,
                                                            pathScopeParams,
                                                            "PATCH",
                                                            path.Key));
                }

                if (path.Value.Post != null)
                {
                    endpoints.Add(ParseIndividualEndpoint(path.Value.Post,
                                                            pathScopeParams,
                                                            "POST",
                                                            path.Key));
                }

                if (path.Value.Put != null)
                {
                    endpoints.Add(ParseIndividualEndpoint(path.Value.Put,
                                                            pathScopeParams,
                                                            "PUT",
                                                            path.Key));
                }
            }

            return endpoints;
        }

        private OpenApiEndpointModel ParseIndividualEndpoint(YamlEndpointModel yamlEndpoint,
                                                                IEnumerable<OpenApiParameter> pathLevelParams,
                                                                string methodName,
                                                                string path)
        {
            var returnedEndpoint = new OpenApiEndpointModel()
            {
                Path = path,
                Method = _enumService.ConvertStringTo<Method>(methodName.ToUpperInvariant()),
                Tags = yamlEndpoint.Tags,
                Description = yamlEndpoint.Description
            };

            var bodySchemaDictEntry = yamlEndpoint.RequestBody?
                                                    .Content?
                                                    .FirstOrDefault();

            if (bodySchemaDictEntry != null)
            {
                returnedEndpoint.RequestBody = new OpenApiRequestOrResponseModel()
                {
                    Type = bodySchemaDictEntry.Value.Key,
                    Content = _typeParser.Parse(_objectParser,
                                                bodySchemaDictEntry.Value
                                                                    .Value
                                                                    .Schema)
                };
            }

            var responseSchemaDictEntry = yamlEndpoint.Responses?
                                                        .FirstOrDefault(r => Regex.IsMatch(r.Key, "^2[0-9]{2}$"))
                                                        .Value?
                                                        .Content?
                                                        .FirstOrDefault();

            if (responseSchemaDictEntry != null)
            {
                returnedEndpoint.SuccessStatusResponse = new OpenApiRequestOrResponseModel()
                {
                    Type = responseSchemaDictEntry.Value.Key,
                    Content = _typeParser.Parse(_objectParser,
                                                responseSchemaDictEntry.Value
                                                                        .Value
                                                                        .Schema)
                };
            }

            var endpointHasParams = yamlEndpoint.Parameters != null && yamlEndpoint.Parameters.Any();
            var pathHasParams = pathLevelParams != null && pathLevelParams.Any();
            if (endpointHasParams || pathHasParams)
            {
                var parametersOut = new List<OpenApiParameter>();

                if (endpointHasParams)
                {
                    parametersOut.AddRange(ParseParameters(yamlEndpoint.Parameters));
                }

                if (pathHasParams)
                {
                    parametersOut.AddRange(pathLevelParams);
                }

                returnedEndpoint.Parameters = parametersOut;
            }

            return returnedEndpoint;
        }

        private List<OpenApiParameter> ParseParameters(IEnumerable<YamlParameterModel> yamlParams)
        {
            var parametersOut = new List<OpenApiParameter>();
            foreach (var param in yamlParams)
            {
                parametersOut.Add(new OpenApiParameter()
                {
                    In = param.In,
                    Schema = _typeParser.Parse(_objectParser,
                                                param.Schema) as OpenApiPrimitiveType,
                    Name = param.Name,
                    Required = param.Required
                });
            }

            return parametersOut;
        }
    }
}
