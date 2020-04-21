using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace TesterCall.Services.UtilsAndWrappers.Interfaces
{
    public interface IModuleBuilderProvider
    {
        ModuleBuilder Builder { get; }
    }
}
