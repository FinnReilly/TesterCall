using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.OpenApi;
using TesterCall.Services.Generation.YamlExtraction.Models;

namespace TesterCall.Services.Generation.YamlExtraction.Interfaces
{
    public interface IOpenApiYamlEndpointsParser
    {
        IEnumerable<OpenApiEndpointModel> Parse(IDictionary<string, 
                                                            IDictionary<string, 
                                                                        YamlEndpointModel>> paths);
    }
}
