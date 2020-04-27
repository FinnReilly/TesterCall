using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TesterCall.Models.OpenApi;

namespace TesterCall.Services.Generation.Interface
{
    public interface ISpecFileToOpenApiModelService
    {
        OpenApiSpecModel ExtractSpec(FileStream file);
    }
}
