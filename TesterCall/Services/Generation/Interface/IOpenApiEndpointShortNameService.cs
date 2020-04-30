using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;

namespace TesterCall.Services.Generation.Interface
{
    public interface IOpenApiEndpointShortNameService
    {
        void CreateOrUpdateShortNames(IEnumerable<OpenApiEndpointModel> endpoints);
    }
}
