﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Interfaces;
using TesterCall.Services.Generation.JsonExtraction.Models;

namespace TesterCall.Services.Generation.JsonExtraction
{
    public class OpenApiSpecObjectParser<TModel> : IOpenApiSpecObjectParser<TModel> 
        where TModel : IOpenApiCatchAllTypeModel<TModel>
    {
        private readonly IOpenApiSpecUmbrellaTypeParser<TModel> _typeParser;

        public OpenApiSpecObjectParser(IOpenApiSpecUmbrellaTypeParser<TModel> openApiUmbrellaJsonTypeParser)
        {
            _typeParser = openApiUmbrellaJsonTypeParser;
        }

        public OpenApiObjectType Parse(TModel model)
        {
            var output = new OpenApiObjectType() 
            {
                Properties = new Dictionary<string, IOpenApiType>()
            };

            if (model.Properties != null)
            {
                foreach (var prop in model.Properties)
                {
                    output.Properties[prop.Key] = _typeParser.Parse(this,
                                                                    prop.Value);
                }
            }

            if (model.AllOf != null && model.AllOf.Any())
            {
                var allOf = new List<IOpenApiType>();
                foreach (var extendedType in model.AllOf)
                {
                    allOf.Add(_typeParser.Parse(this,
                                                extendedType));
                }
                output.AllOf = allOf;
            }

            return output;
        }
    }
}
