using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.YamlExtraction.Interfaces;
using TesterCall.Services.Generation.YamlExtraction.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TesterCall.Services.Generation.YamlExtraction
{
    public class YamlFileToOpenApiModelService : IYamlFileToOpenApiModelService
    {
        private readonly IOpenApiSpecObjectParser<YamlCatchAllTypeModel> _objectParser;
        private readonly IOpenApiSpecUmbrellaTypeParser<YamlCatchAllTypeModel> _typeParser;
        private readonly IOpenApiYamlEndpointsParser _endpointsParser;
        private readonly IOpenApiEndpointShortNameService _shortNameService;

        public YamlFileToOpenApiModelService(IOpenApiSpecObjectParser<YamlCatchAllTypeModel> openApiSpecObjectParser,
                                            IOpenApiSpecUmbrellaTypeParser<YamlCatchAllTypeModel> openApiSpecUmbrellaTypeParser,
                                            IOpenApiYamlEndpointsParser openApiYamlEndpointsParser,
                                            IOpenApiEndpointShortNameService openApiEndpointShortNameService)
        {
            _objectParser = openApiSpecObjectParser;
            _typeParser = openApiSpecUmbrellaTypeParser;
            _endpointsParser = openApiYamlEndpointsParser;
            _shortNameService = openApiEndpointShortNameService;
        }

        public OpenApiSpecModel ExtractSpec(FileStream file,
                                            string overwriteApiTitle)
        {
            var output = new OpenApiSpecModel();
            var deserializer = new DeserializerBuilder()
                                        .WithNamingConvention(new CamelCaseNamingConvention())
                                        .IgnoreUnmatchedProperties()
                                        .Build();

            YamlSpecModel yamlModel;
            using (var reader = new StreamReader(file))
            {
                yamlModel = deserializer.Deserialize<YamlSpecModel>(reader);
            }

            output.Info = yamlModel.Info;
            if (!string.IsNullOrEmpty(overwriteApiTitle))
            {
                output.Info.Title = overwriteApiTitle;
            }

            output.Definitions = new Dictionary<string, IOpenApiType>();
            foreach (var definedType in yamlModel.Components.Schemas)
            {
                output.Definitions[definedType.Key] = _typeParser.Parse(_objectParser,
                                                                    definedType.Value);
            }

            if (yamlModel.Paths != null)
            {
                output.Endpoints = _endpointsParser.Parse(yamlModel.Paths);
                _shortNameService.CreateOrUpdateShortNames(output.Endpoints);
            }

            return output;
        }
    }
}
