using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Endpoints;
using TesterCall.Models.OpenApi;

namespace TesterCall.Services.Generation.Interface
{
    public interface IOpenApiEndpointToEndpointService
    {
        Endpoint GenerateEndpoint(OpenApiEndpointModel inputModel);
    }
}
