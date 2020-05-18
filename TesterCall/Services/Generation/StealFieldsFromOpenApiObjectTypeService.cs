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
                                IObjectsProcessingKeyStore keyStore,
                                TypeBuilder typeBuilder, 
                                IEnumerable<IOpenApiType> extendedTypes,
                                IDictionary<string, IOpenApiType> definitions)
        {
            foreach (var openApiType in extendedTypes)
            {
                Type donorType = typeof(object);
                donorType = _typeResolver.GetType(openApiObjectToTypeService,
                                                    keyStore,
                                                    openApiType,
                                                    definitions,
                                                    $"PropertyHolder_{Guid.NewGuid()}");

                if (donorType == typeof(object))
                {
                    throw new NotSupportedException($"Type in All Of property for {typeBuilder.Name} not recognised");
                }

                foreach (var field in donorType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    typeBuilder.DefineField(field.Name,
                                            field.FieldType,
                                            FieldAttributes.Public);
                }
            }
        }
    }
}
