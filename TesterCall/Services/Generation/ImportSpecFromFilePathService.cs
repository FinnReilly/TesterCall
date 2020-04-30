using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;

namespace TesterCall.Services.Generation
{
    public class ImportSpecFromFilePathService : IImportSpecFromFilePathService
    {
        private readonly IJsonFileToOpenApiModelService _jsonParseService;
        private readonly IOpenApiSpecModelToGeneratedTypesService _typeGenerationService;

        public ImportSpecFromFilePathService(IJsonFileToOpenApiModelService jsonFileToOpenApiModelService,
                                            IOpenApiSpecModelToGeneratedTypesService openApiSpecModelToGeneratedTypesService)
        {
            _jsonParseService = jsonFileToOpenApiModelService;
            _typeGenerationService = openApiSpecModelToGeneratedTypesService;
        }

        public void Import(string filePath)
        {
            if (!filePath.Contains("."))
            {
                throw new ArgumentException("No file extension detected");
            }

            var fileExtension = filePath.Split('.').Last();

            if (fileExtension == "yml" || fileExtension == "yaml")
            {
                // write some yaml services
            }

            if (fileExtension == "json")
            {
                var fileStream = File.OpenRead(filePath);
                var openApiModel = _jsonParseService.ExtractSpec(fileStream);

                _typeGenerationService.Generate(openApiModel);
            }

            throw new FormatException("Can only accept .yml/.yaml or .json Open Api/Swagger files");
        }
    }
}
