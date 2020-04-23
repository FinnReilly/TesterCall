using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Extensions;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Services.Generation
{
    public class OpenApiPrimitiveToTypeService : IOpenApiPrimitiveToTypeService
    {
        private readonly IOpenApiEnumToTypeService _enumService;

        public OpenApiPrimitiveToTypeService(IOpenApiEnumToTypeService openApiEnumToTypeService)
        {
            _enumService = openApiEnumToTypeService;
        }

        public Type GetType(OpenApiPrimitiveType primitive)
        {
            var simpleTypeDict = new Dictionary<string, Type>()
            {
                { "integer", typeof(int) },
                { "float", typeof(double) },
                { "boolean", typeof(bool) }
            };

            var stringTypeFormatType = new Dictionary<string, Type>()
            {
                { "date-time", typeof(DateTime) }
            };

            if (primitive.Matches<OpenApiEnumType>())
            {
                return _enumService.GetType((OpenApiEnumType)primitive);
            }

            if (simpleTypeDict.TryGetValue(primitive.Type, out var mappedType))
            {
                return mappedType;
            }

            if (primitive.Type == "string")
            {
                if (primitive.Format != null 
                    && stringTypeFormatType.TryGetValue(primitive.Format, 
                                                        out var mappedFormatType))
                {
                    return mappedFormatType;
                }

                return typeof(string);
            }

            throw new NotSupportedException($"No support available for primitive with type = {primitive.Type}" +
                $" and format = {primitive.Format}");
        }
    }
}
