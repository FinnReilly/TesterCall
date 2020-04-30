using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesterCall.Services.Generation.JsonExtraction.Models.Extensions
{
    public static class JsonCatchAllTypeModelExtensions
    {
        private static List<string> _primitiveTypes = new List<string>()
        {
            "string",
            "integer",
            "number",
            "boolean"
        };

        public static bool IsPrimitive(this JsonCatchAllTypeModel model)
        {
            if (!string.IsNullOrEmpty(model.Type)
                && _primitiveTypes.Contains(model.Type.ToLowerInvariant()))
            {
                return true;
            }

            return false;
        }

        public static bool IsEnum(this JsonCatchAllTypeModel model)
        {
            if (model.Enum != null && model.Enum.Any())
            {
                return true;
            }

            return false;
        }

        public static bool IsArray(this JsonCatchAllTypeModel model)
        {
            if (!string.IsNullOrEmpty(model.Type)
                && model.Type.ToLowerInvariant() == "array"
                && model.Items != null)
            {
                return true;
            }

            return false;
        }

        public static bool IsObject(this JsonCatchAllTypeModel model)
        {
            if (!string.IsNullOrEmpty(model.Type)
                && model.Type.ToLowerInvariant() == "object"
                && ((model.Properties != null && model.Properties.Any())
                    || (model.AllOf != null && model.AllOf.Any())))
            {
                return true;
            }

            return false;
        }

        public static bool IsReference(this JsonCatchAllTypeModel model)
        {
            if (!string.IsNullOrEmpty(model.Reference))
            {
                return true;
            }

            return false;
        }
    }
}
