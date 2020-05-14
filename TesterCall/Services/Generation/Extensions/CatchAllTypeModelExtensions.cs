using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Services.Generation.Interface;

namespace TesterCall.Services.Generation.JsonExtraction.Models.Extensions
{
    public static class CatchAllTypeModelExtensions
    {
        private static List<string> _primitiveTypes = new List<string>()
        {
            "string",
            "integer",
            "number",
            "boolean"
        };

        public static bool IsPrimitive<TModel>(this TModel model)
            where TModel : IOpenApiCatchAllTypeModel<TModel>
        {
            if (!string.IsNullOrEmpty(model.Type)
                && _primitiveTypes.Contains(model.Type.ToLowerInvariant()))
            {
                return true;
            }

            return false;
        }

        public static bool IsEnum<TModel>(this TModel model)
            where TModel : IOpenApiCatchAllTypeModel<TModel>
        {
            if (model.Enum != null && model.Enum.Any())
            {
                return true;
            }

            return false;
        }

        public static bool IsArray<TModel>(this TModel model)
            where TModel : IOpenApiCatchAllTypeModel<TModel>
        {
            if (!string.IsNullOrEmpty(model.Type)
                && model.Type.ToLowerInvariant() == "array"
                && model.Items != null)
            {
                return true;
            }

            return false;
        }

        public static bool IsObject<TModel>(this TModel model)
            where TModel : IOpenApiCatchAllTypeModel<TModel>
        {
            if ((model.Properties != null && model.Properties.Any())
                    || (model.AllOf != null && model.AllOf.Any()))
            {
                return true;
            }

            return false;
        }

        public static bool IsReference<TModel>(this TModel model)
            where TModel : IOpenApiCatchAllTypeModel<TModel>
        {
            if (!string.IsNullOrEmpty(model.Reference))
            {
                return true;
            }

            return false;
        }
    }
}
