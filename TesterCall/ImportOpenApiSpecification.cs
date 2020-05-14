using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using TesterCall.Powershell;
using TesterCall.Services.Generation;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction;
using TesterCall.Services.Generation.JsonExtraction.Models;
using TesterCall.Services.Generation.YamlExtraction;
using TesterCall.Services.Generation.YamlExtraction.Models;
using TesterCall.Services.Usage.Formatting;
using TesterCall.Services.UtilsAndWrappers;

namespace TesterCall
{
    [Cmdlet(VerbsData.Import, "OpenApiSpecification")]
    public class ImportOpenApiSpecification : TesterCallCmdlet
    {
        private IImportSpecFromFilePathService _specImportService;
        private string _pwd;

        [Parameter(Mandatory = true,
                    ValueFromPipeline = true,
                    Position = 0)]
        public string Path { get; set; }

        [Alias("Name")]
        [Parameter(Mandatory = false,
                    Position = 1,
                    ValueFromPipelineByPropertyName = true)]
        public string Title { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            var filePathFormatService = new FilePathFormattingService();
            var jsonTypeParser = new OpenApiSpecUmbrellaTypeParser<JsonCatchAllTypeModel>();
            var jsonObjectParser = new OpenApiSpecObjectParser<JsonCatchAllTypeModel>(jsonTypeParser);
            var yamlTypeParser = new OpenApiSpecUmbrellaTypeParser<YamlCatchAllTypeModel>();
            var yamlObjectParser = new OpenApiSpecObjectParser<YamlCatchAllTypeModel>(yamlTypeParser);
            var lastTokenService = new LastTokenInPathService();
            var enumFromStringService = new EnumFromStringService();
            var shortNameService = new OpenApiEndpointShortNameService(lastTokenService);

            var moduleBuilderProvider = new ModuleBuilderProvider();
            var openApiPrimitiveService = new OpenApiPrimitiveToTypeService(new OpenApiEnumToTypeService(moduleBuilderProvider));
            var openApiTypeResolver = new OpenApiUmbrellaTypeResolver(openApiPrimitiveService,
                                                                        new OpenApiReferenceToTypeService(lastTokenService));
            var openApiObjectToTypeService = new OpenApiObjectToTypeService(openApiTypeResolver,
                                                                            new StealFieldsFromOpenApiObjectTypeService(openApiTypeResolver),
                                                                            moduleBuilderProvider);
            //establish current ps session working directory
            using (var session = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                session.AddCommand("get-location");
                _pwd = session.Invoke<PathInfo>()
                                .FirstOrDefault()?.Path;
            }

            _specImportService = new ImportSpecFromFilePathService(filePathFormatService,
                                                                        new JsonFileToOpenApiModelService(jsonObjectParser,
                                                                                                        jsonTypeParser,
                                                                                                        new OpenApiJsonEndpointsParser(jsonTypeParser,
                                                                                                                                        jsonObjectParser,
                                                                                                                                        enumFromStringService),
                                                                                                        shortNameService),
                                                                        new YamlFileToOpenApiModelService(yamlObjectParser,
                                                                                                        yamlTypeParser,
                                                                                                        new OpenApiYamlEndpointsParser(yamlTypeParser,
                                                                                                                                        yamlObjectParser,
                                                                                                                                        enumFromStringService),
                                                                                                        shortNameService),
                                                                        new OpenApiSpecModelToGeneratedTypesService(openApiObjectToTypeService,
                                                                                                                    openApiTypeResolver,
                                                                                                                    new OpenApiEndpointToEndpointService(openApiTypeResolver,
                                                                                                                                                        openApiPrimitiveService,
                                                                                                                                                        openApiObjectToTypeService)));
        }

        protected override void ProcessRecord()
        {
            var endpoints  = _specImportService.Import(Path, 
                                                        Title,
                                                        _pwd);

            WriteObject(endpoints, enumerateCollection: true);
        }
    }
}
