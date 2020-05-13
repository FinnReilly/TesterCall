using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using TesterCall.Powershell;
using TesterCall.Services.Generation;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction;
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

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            var filePathFormatService = new FilePathFormattingService();
            var jsonTypeParser = new OpenApiSpecUmbrellaTypeParser();
            var jsonObjectParser = new OpenApiSpecObjectParser(jsonTypeParser);
            var lastTokenService = new LastTokenInPathService();

            var moduleBuilderProvider = new ModuleBuilderProvider();
            var openApiPrimitiveService = new OpenApiPrimitiveToTypeService(new OpenApiEnumToTypeService(moduleBuilderProvider));
            var openApiTypeResolver = new OpenApiUmbrellaTypeResolver(openApiPrimitiveService,
                                                                        new OpenApiReferenceToTypeService(lastTokenService));
            var openApiObjectToTypeService = new OpenApiObjectToTypeService(openApiTypeResolver,
                                                                            new StealFieldsFromOpenApiObjectTypeService(openApiTypeResolver),
                                                                            moduleBuilderProvider);
            using (var session = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                session.AddCommand("get-location");
                _pwd = session.Invoke<PathInfo>()
                                .FirstOrDefault()?.Path;
            }

            _specImportService = new ImportSpecFromFilePathService(filePathFormatService,
                                                                        new JsonFileToOpenApiModelService(jsonObjectParser,
                                                                                                        new OpenApiJsonEndpointsParser(jsonTypeParser,
                                                                                                                                        jsonObjectParser,
                                                                                                                                        new EnumFromStringService()),
                                                                                                        new OpenApiEndpointShortNameService(lastTokenService)),
                                                                        new OpenApiSpecModelToGeneratedTypesService(openApiObjectToTypeService,
                                                                                                                    new OpenApiEndpointToEndpointService(openApiTypeResolver,
                                                                                                                                                        openApiPrimitiveService,
                                                                                                                                                        openApiObjectToTypeService)));
        }

        protected override void ProcessRecord()
        {
            var endpoints  = _specImportService.Import(Path, _pwd);

            WriteObject(endpoints, enumerateCollection: true);
        }
    }
}
