using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Endpoints;

namespace TesterCall.Services.Generation.Interface
{
    public interface IImportSpecFromFilePathService
    {
        IEnumerable<Endpoint> Import(string filePath,
                                    string pwd);
    }
}
