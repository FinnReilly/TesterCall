using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TesterCall.Models.OpenApi.Interfaces;

namespace TesterCall.Services.Generation.Interface
{
    public interface IStealFieldsFromOpenApiObjectTypesService
    {
        void AddFields(TypeBuilder typeBuilder, IEnumerable<IOpenApiType> extendedTypes);
    }
}
