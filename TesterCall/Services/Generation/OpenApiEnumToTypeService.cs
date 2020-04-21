using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.Generation
{
    public class OpenApiEnumToTypeService : IOpenApiEnumToTypeService
    {
        private readonly IModuleBuilderProvider _module;

        public OpenApiEnumToTypeService(IModuleBuilderProvider moduleBuilderProvider)
        {
            _module = moduleBuilderProvider;
        }

        public Type GetType(OpenApiEnumType enumeration)
        {
            var enumBuilder = _module.Builder.DefineEnum("Name",
                                                        TypeAttributes.Public,
                                                        typeof(int));
            var i = 0;
            foreach (var value in enumeration.Enum)
            {
                enumBuilder.DefineLiteral(value, i);
                i++;
            }

            return enumBuilder.CreateTypeInfo();
        }
    }
}
