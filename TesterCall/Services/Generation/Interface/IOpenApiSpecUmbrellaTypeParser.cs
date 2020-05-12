using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;
using TesterCall.Services.Generation.Interface;
using TesterCall.Services.Generation.JsonExtraction.Models;

namespace TesterCall.Services.Generation.JsonExtraction.Interfaces
{
    public interface IOpenApiSpecUmbrellaTypeParser<TModel>
        where TModel : IOpenApiCatchAllTypeModel<TModel>
    {
        IOpenApiType Parse(IOpenApiSpecObjectParser<TModel> objectParser,
                            TModel model);
    }
}
