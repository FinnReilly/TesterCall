using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Models;

namespace TesterCall.Services.Generation.JsonExtraction.Interfaces
{
    public interface IOpenApiSpecObjectParser<TModel>
        where TModel : IOpenApiCatchAllTypeModel<TModel>
    {
        OpenApiObjectType Parse(TModel model);
    }
}
