using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
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
                                                                    IDictionary<string, 
                                                                                YamlEndpointModel>> paths)
        {
            throw new NotImplementedException();
        }
    }
}
