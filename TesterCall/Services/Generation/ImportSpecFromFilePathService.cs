using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models.Endpoints;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.YamlExtraction.Interfaces;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Generation
{
    public class ImportSpecFromFilePathService : IImportSpecFromFilePathService
    {
        private readonly IFilePathFormattingService _filePathService;
        private readonly IJsonFileToOpenApiModelService _jsonParseService;
        private readonly IYamlFileToOpenApiModelService _yamlParseService;
        private readonly IOpenApiSpecModelToGeneratedTypesService _typeGenerationService;

        public ImportSpecFromFilePathService(IFilePathFormattingService filePathFormattingService,
                                            IJsonFileToOpenApiModelService jsonFileToOpenApiModelService,
                                            IYamlFileToOpenApiModelService yamlFileToOpenApiModelService,
                                            IOpenApiSpecModelToGeneratedTypesService openApiSpecModelToGeneratedTypesService)
        {
            _filePathService = filePathFormattingService;
            _jsonParseService = jsonFileToOpenApiModelService;
            _yamlParseService = yamlFileToOpenApiModelService;
            _typeGenerationService = openApiSpecModelToGeneratedTypesService;
        }

        public IEnumerable<Endpoint> Import(string filePath,
                                            string overwriteTitle,
                                            string pwd)
        {
            // handle relative paths
            filePath = _filePathService.FormatPath(filePath, pwd);

            if (!filePath.Contains("."))
            {
                throw new ArgumentException("No file extension detected");
            }

            var fileExtension = filePath.Split('.').Last();
            var apiId = "";

            OpenApiSpecModel openApiModel = null;
            var fileStream = File.OpenRead(filePath);
            switch (fileExtension) 
            {
                case "yml":
                case "yaml":
                    openApiModel = _yamlParseService.ExtractSpec(fileStream,
                                                                    overwriteTitle);
                    break;
                case "json":
                    openApiModel = _jsonParseService.ExtractSpec(fileStream,
                                                                    overwriteTitle);
                    break;
                default:
                    throw new FormatException("Can only accept .yml/.yaml or .json Open Api/Swagger files");
            }

            apiId = openApiModel.Info.Title;
            _typeGenerationService.Generate(openApiModel);

            return CurrentEndpointHolder.Endpoints.Values.Where(e => e.ApiId == apiId);
        }
    }
}
