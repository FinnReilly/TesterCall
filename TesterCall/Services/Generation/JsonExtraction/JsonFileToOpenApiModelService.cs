using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TesterCall.Enums;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Models;
using TesterCall.Services.Generation.JsonExtraction.Models.Enums;
using TesterCall.Services.Usage.Formatting.Interfaces;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Generation.JsonExtraction
{
    public class JsonFileToOpenApiModelService : IJsonFileToOpenApiModelService
    {
        private readonly IOpenApiJsonObjectParser _objectParser;
        private readonly IOpenApiJsonEndpointsParser _endpointsParser;
        private readonly IOpenApiEndpointShortNameService _shortNameService;

        public JsonFileToOpenApiModelService(IOpenApiJsonObjectParser openApiJsonObjectParser,
                                            IOpenApiJsonEndpointsParser openApiJsonEndpointsParser,
                                            IOpenApiEndpointShortNameService openApiEndpointShortNameService)
        {
            _objectParser = openApiJsonObjectParser;
            _endpointsParser = openApiJsonEndpointsParser;
            _shortNameService = openApiEndpointShortNameService;
        }

        public OpenApiSpecModel ExtractSpec(FileStream file)
        {
            var output = new OpenApiSpecModel();
            var serializer = new JsonSerializer();
            serializer.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;

            JsonSpecModel jsonModel;
            using (var reader = new StreamReader(file)) {
                using (var jsonReader = new JsonTextReader(reader)) {
                    jsonModel = serializer.Deserialize<JsonSpecModel>(jsonReader);
                }
            }

            output.Info = jsonModel.Info;

            output.Definitions = new Dictionary<string, OpenApiObjectType>();
            foreach (var definedType in jsonModel.Definitions)
            {
                output.Definitions[definedType.Key] = _objectParser.Parse(definedType.Value);
            }

            output.Endpoints = _endpointsParser.Parse(jsonModel.Paths);
            _shortNameService.CreateOrUpdateShortNames(output.Endpoints);

            return output;
        }
    }
}
