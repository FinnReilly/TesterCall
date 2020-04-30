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
        private readonly IOpenApiUmbrellaTypeResolver _typeResolver;

        public StealFieldsFromOpenApiObjectTypeService(IOpenApiUmbrellaTypeResolver openApiUmbrellaTypeResolver)
        {
            _typeResolver = openApiUmbrellaTypeResolver;
        }

        public void AddFields(IOpenApiObjectToTypeService openApiObjectToTypeService,
                                TypeBuilder typeBuilder, 
                                IEnumerable<IOpenApiType> extendedTypes,
                                IDictionary<string, OpenApiObjectType> definitions)
        {
            foreach (var openApiType in extendedTypes)
            {
                Type donorType = typeof(object);
                donorType = _typeResolver.GetType(openApiObjectToTypeService,
                                                    openApiType,
                                                    definitions,
                                                    $"PropertyHolder_{Guid.NewGuid()}");

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
