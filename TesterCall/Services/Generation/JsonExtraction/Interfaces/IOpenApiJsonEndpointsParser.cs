using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.JsonExtraction.Models;

namespace TesterCall.Services.Generation.JsonExtraction.Interfaces
{
    public interface IOpenApiJsonEndpointsParser
    {
        IEnumerable<OpenApiEndpointModel> Parse(IDictionary<string, 
                                                            IDictionary<string, 
                                                                        JsonEndpointModel>> paths);
    }
}
