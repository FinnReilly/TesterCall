using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.YamlExtraction.Interfaces;
using TesterCall.Services.Generation.YamlExtraction.Models;
using YamlDotNet.Serialization;

namespace TesterCall.Services.Generation.YamlExtraction
{
    public class YamlFileToOpenApiModelService : IYamlFileToOpenApiModelService
    {
        private readonly IOpenApiSpecObjectParser<YamlCatchAllTypeModel> _objectParser;
        private readonly IOpenApiYamlEndpointsParser _endpointsParser;
        private readonly IOpenApiEndpointShortNameService _shortNameService;

        public YamlFileToOpenApiModelService(IOpenApiSpecObjectParser<YamlCatchAllTypeModel> openApiSpecObjectParser,
                                            IOpenApiYamlEndpointsParser openApiYamlEndpointsParser,
                                            IOpenApiEndpointShortNameService openApiEndpointShortNameService)
        {
            _objectParser = openApiSpecObjectParser;
            _endpointsParser = openApiYamlEndpointsParser;
            _shortNameService = openApiEndpointShortNameService;
        }

        public OpenApiSpecModel ExtractSpec(FileStream file)
        {
            var output = new OpenApiSpecModel();
            var deserializer = new DeserializerBuilder()
                                        .Build();

            YamlSpecModel yamlModel;
            using (var reader = new StreamReader(file))
            {
                yamlModel = deserializer.Deserialize<YamlSpecModel>(reader);
            }

            output.Info = yamlModel.Info;

            output.Definitions = new Dictionary<string, OpenApiObjectType>();
            foreach (var definedType in yamlModel.Components.Schemas)
            {
                output.Definitions[definedType.Key] = _objectParser.Parse(definedType.Value);
            }

            output.Endpoints = _endpointsParser.Parse(yamlModel.Paths);
            _shortNameService.CreateOrUpdateShortNames(output.Endpoints);

            return output;
        }
    }
}
