using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Generation
{
    public class OpenApiObjectToTypeService : IOpenApiObjectToTypeService
    {
        private readonly IOpenApiUmbrellaTypeResolver _typeResolver;
        private readonly IStealFieldsFromOpenApiObjectTypesService _fieldStealer;
        private readonly IObjectsProcessingKeyStore _objectsKeyStore;
        private readonly IModuleBuilderProvider _module;

        public OpenApiObjectToTypeService(IOpenApiUmbrellaTypeResolver openApiUmbrellaTypeResolver,
                                            IStealFieldsFromOpenApiObjectTypesService stealFieldsFromOpenApiObjectTypesService,
                                            IObjectsProcessingKeyStore objectsProcessingStore,
                                            IModuleBuilderProvider moduleBuilderProvider)
        {
            _typeResolver = openApiUmbrellaTypeResolver;
            _fieldStealer = stealFieldsFromOpenApiObjectTypesService;
            _objectsKeyStore = objectsProcessingStore;
            _module = moduleBuilderProvider;
        }

        public Type GetType(OpenApiObjectType inputObject, 
                            OpenApiDefinitionsModel definitions,
                            string name)
        {
            _objectsKeyStore.ThrowIfPresent(name);

            var typeBuilder = _module.Builder.DefineType(name,
                                                        TypeAttributes.Public);
            _objectsKeyStore.AddPresent(name);

            foreach (var property in inputObject.Properties)
            {
                var propName = property.Key;
                var propType = _typeResolver.GetType(this,
                                                    property.Value,
                                                    definitions,
                                                    $"{name}_{propName}");

                DefinePublicField(typeBuilder,
                                    propType,
                                    propName);
            }

            if (inputObject.AllOf != null && inputObject.AllOf.Any())
            {
                _fieldStealer.AddFields(this,
                                        typeBuilder, 
                                        inputObject.AllOf,
                                        definitions);
            }

            _objectsKeyStore.RemovePresent(name);

            return typeBuilder.CreateTypeInfo();
        }

        private void DefinePublicField(TypeBuilder typeBuilder,
                                        Type type,
                                        string name)
        {
            typeBuilder.DefineField(name, 
                                    type, 
                                    FieldAttributes.Public);
        }
    }
}
