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
        private readonly IOpenApiPrimitiveToTypeService _primitiveService;
        private readonly IOpenApiReferenceToTypeService _referenceService;
        private readonly IStealFieldsFromOpenApiObjectTypesService _fieldStealer;
        private readonly IModuleBuilderProvider _module;

        public OpenApiObjectToTypeService(IOpenApiPrimitiveToTypeService openApiPrimitiveToTypeService,
                                            IOpenApiReferenceToTypeService openApiReferenceToTypeService,
                                            IStealFieldsFromOpenApiObjectTypesService stealFieldsFromOpenApiObjectTypesService,
                                            IModuleBuilderProvider moduleBuilderProvider)
        {
            _primitiveService = openApiPrimitiveToTypeService;
            _referenceService = openApiReferenceToTypeService;
            _fieldStealer = stealFieldsFromOpenApiObjectTypesService;
            _module = moduleBuilderProvider;
        }

        public Type GetType(OpenApiObjectType inputObject, string name)
        {
            var typeBuilder = _module.Builder.DefineType(name,
                                                        TypeAttributes.Public);

            foreach (var property in inputObject.Properties)
            {
                if (property.Value.GetType() == typeof(OpenApiPrimitiveType))
                {
                    var primType = _primitiveService.GetType((OpenApiPrimitiveType)property.Value);
                    var primName = property.Key;

                    DefinePublicField(typeBuilder, primType, primName);
                }

                if (property.Value.GetType() == typeof(OpenApiReferencedType))
                {
                    var refType = _referenceService.GetType(this,
                                                            (OpenApiReferencedType)property.Value,
                                                            inputObject.Definitions);
                    var refName = property.Key;

                    DefinePublicField(typeBuilder, refType, refName);
                }

                if (property.Value.GetType() == typeof(OpenApiObjectType))
                {
                    var objectName = property.Key;
                    var objectTypeName = $"{name}_{objectName}";
                    var objectType = GetType((OpenApiObjectType)property.Value, 
                                            objectTypeName);

                    DefinePublicField(typeBuilder, objectType, objectName);
                }
            }

            if (inputObject.AllOf != null && inputObject.AllOf.Count() > 0)
            {
                _fieldStealer.AddFields(typeBuilder, inputObject.AllOf);
            }

            return typeBuilder.CreateTypeInfo();
        }

        private void DefinePublicField(TypeBuilder typeBuilder,
                                        Type type,
                                        string name)
        {
            typeBuilder.DefineField(name, type, FieldAttributes.Public);
        }
    }
}
