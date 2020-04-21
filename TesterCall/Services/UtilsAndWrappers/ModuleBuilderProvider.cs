using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TesterCall.Services.UtilsAndWrappers.Interfaces;

namespace TesterCall.Services.UtilsAndWrappers
{
    public class ModuleBuilderProvider : IModuleBuilderProvider
    {
        ModuleBuilder _builder;

        public ModuleBuilderProvider()
        {
            var assemblyName = new AssemblyName();
            assemblyName.Name = "Generated";

            var builder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _builder = builder.DefineDynamicModule("GenerationModule");
        }

        public ModuleBuilder Builder => _builder;
    }
}
