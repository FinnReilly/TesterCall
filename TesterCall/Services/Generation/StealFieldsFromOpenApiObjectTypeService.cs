using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Services.Generation
{
    public class StealFieldsFromOpenApiObjectTypeService : IStealFieldsFromOpenApiObjectTypesService
    {
        private readonly IOpenApiObjectToTypeService _objectService;
        private readonly IOpenApiReferenceToTypeService _referenceService;

        public StealFieldsFromOpenApiObjectTypeService(IOpenApiObjectToTypeService openApiObjectToTypeService,
                                                        IOpenApiReferenceToTypeService openApiReferenceToTypeService)
        {
            _objectService = openApiObjectToTypeService;
            _referenceService = openApiReferenceToTypeService;
        }

        public void AddFields(TypeBuilder typeBuilder, 
                                IEnumerable<IOpenApiType> extendedTypes)
        {
            foreach (var openApiType in extendedTypes)
            {
                Type donorType = typeof(object);
                if (openApiType.GetType() == typeof(OpenApiObjectType))
                {
                    donorType = _objectService.GetType((OpenApiObjectType)openApiType,
                                                        $"PropertiesHolder_{Guid.NewGuid()}");
                }
                if (openApiType.GetType() == typeof(OpenApiReferencedType))
                {
                    var asReferenced = (OpenApiReferencedType)openApiType;
                    donorType = _referenceService.GetType(_objectService,
                                                            asReferenced,
                                                            asReferenced.Object.Definitions);
                }

                if (donorType == typeof(object))
                {
                    throw new NotSupportedException($"Type in All Of property for {typeBuilder.Name} not recognised");
                }

                foreach (var property in donorType.GetProperties(BindingFlags.Public))
                {
                    typeBuilder.DefineField(property.Name,
                                            property.PropertyType,
                                            FieldAttributes.Public);
                }
            }
        }
    }
}
