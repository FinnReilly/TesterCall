using System;
using System.Collections.Generic;
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

        [Parameter(Mandatory = true,
                    ValueFromPipeline = true,
                    Position = 0)]
        public string Path { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            var jsonTypeParser = new OpenApiUmbrellaJsonTypeParser();
            var jsonObjectParser = new OpenApiJsonObjectParser(jsonTypeParser);
            var lastTokenService = new LastTokenInPathService();

            var moduleBuilderProvider = new ModuleBuilderProvider();
            var objectsProcessingKeyStore = new ObjectsProcessingKeyStore();
            var openApiPrimitiveService = new OpenApiPrimitiveToTypeService(new OpenApiEnumToTypeService(moduleBuilderProvider));
            var openApiTypeResolver = new OpenApiUmbrellaTypeResolver(openApiPrimitiveService,
                                                                        new OpenApiReferenceToTypeService(lastTokenService,
                                                                                                            objectsProcessingKeyStore));
            var openApiObjectToTypeService = new OpenApiObjectToTypeService(openApiTypeResolver,
                                                                            new StealFieldsFromOpenApiObjectTypeService(openApiTypeResolver),
                                                                            objectsProcessingKeyStore,
                                                                            moduleBuilderProvider);

            _specImportService = new ImportSpecFromFilePathService(new JsonFileToOpenApiModelService(jsonObjectParser,
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
            _specImportService.Import(Path);
        }
    }
}
