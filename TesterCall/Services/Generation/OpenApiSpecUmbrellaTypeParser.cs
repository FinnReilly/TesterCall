using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Models;
using TesterCall.Services.Generation.JsonExtraction.Models.Extensions;

namespace TesterCall.Services.Generation.JsonExtraction
{
    public class OpenApiSpecUmbrellaTypeParser<TModel> : IOpenApiSpecUmbrellaTypeParser<TModel>
        where TModel : IOpenApiCatchAllTypeModel<TModel>
    {
        public IOpenApiType Parse(IOpenApiSpecObjectParser<TModel> objectParser, 
                                TModel model)
        {
            if (model.IsPrimitive())
            {
                if (model.IsEnum())
                {
                    return new OpenApiEnumType()
                    {
                        Enum = model.Enum.ToArray()
                    };
                }

                return new OpenApiPrimitiveType()
                {
                    Type = model.Type,
                    Format = model.Format
                };
            }

            if (model.IsObject())
            {
                return objectParser.Parse(model);
            }

            if (model.IsReference())
            {
                return new OpenApiReferencedType()
                {
                    Type = model.Reference
                };
            }

            if (model.IsArray())
            {
                return new OpenApiArrayType()
                {
                    Items = Parse(objectParser, 
                                model.Items)
                };
            }

            throw new NotSupportedException("Error in parsing representation of OpenApi type");
        }
    }
}
