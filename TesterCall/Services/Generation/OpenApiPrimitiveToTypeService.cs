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

        public Type GetType(OpenApiPrimitiveType primitive,
                            string nameIfEnum)
        {
            var simpleTypeDict = new Dictionary<string, Type>()
            {
                { "integer", typeof(int?) },
                { "float", typeof(double?) },
                { "boolean", typeof(bool?) }
            };

            var stringTypeFormatType = new Dictionary<string, Type>()
            {
                { "date-time", typeof(DateTime?) }
            };

            var numberTypeFormatType = new Dictionary<string, Type>()
            {
                { "double", typeof(double?) }
            };

            if (primitive.Matches<OpenApiEnumType>())
            {
                return typeof(Nullable<>)
                        .MakeGenericType(_enumService.GetType((OpenApiEnumType)primitive,
                                        nameIfEnum));
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

            if (primitive.Type == "number")
            {
                if (primitive.Format != null
                    && numberTypeFormatType.TryGetValue(primitive.Format,
                                                        out var mappedNumberFormatType))
                {
                    return mappedNumberFormatType;
                }

                return typeof(int?);
            }

            throw new NotSupportedException($"No support available for primitive with type = {primitive.Type}" +
                $" and format = {primitive.Format}");
        }
    }
}
