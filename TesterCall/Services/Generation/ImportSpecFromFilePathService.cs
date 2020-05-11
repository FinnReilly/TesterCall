using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TesterCall.Holders;
using TesterCall.Models.Endpoints;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Usage.Formatting.Interfaces;

namespace TesterCall.Services.Generation
{
    public class ImportSpecFromFilePathService : IImportSpecFromFilePathService
    {
        private readonly IFilePathFormattingService _filePathService;
        private readonly IJsonFileToOpenApiModelService _jsonParseService;
        private readonly IOpenApiSpecModelToGeneratedTypesService _typeGenerationService;

        public ImportSpecFromFilePathService(IFilePathFormattingService filePathFormattingService,
                                            IJsonFileToOpenApiModelService jsonFileToOpenApiModelService,
                                            IOpenApiSpecModelToGeneratedTypesService openApiSpecModelToGeneratedTypesService)
        {
            _filePathService = filePathFormattingService;
            _jsonParseService = jsonFileToOpenApiModelService;
            _typeGenerationService = openApiSpecModelToGeneratedTypesService;
        }

        public IEnumerable<Endpoint> Import(string filePath,
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

            switch (fileExtension) 
            {
                case "yml":
                case "yaml":
                    //put some yaml code here
                    break;
                case "json":
                    var fileStream = File.OpenRead(filePath);
                    var openApiModel = _jsonParseService.ExtractSpec(fileStream);
                    apiId = openApiModel.Info.Title;

                    _typeGenerationService.Generate(openApiModel);
                    break;
                default:
                    throw new FormatException("Can only accept .yml/.yaml or .json Open Api/Swagger files");
            }

            return CurrentEndpointHolder.Endpoints.Values.Where(e => e.ApiId == apiId);
        }
    }
}
