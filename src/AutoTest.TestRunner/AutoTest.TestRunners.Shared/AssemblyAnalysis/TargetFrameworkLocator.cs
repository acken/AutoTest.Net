using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public class TargetFrameworkLocator : ITargetFrameworkLocator
    {
        public Version Locate(string assembly)
        {
            var asm = AssemblyDefinition.ReadAssembly(assembly);
            var runtime = asm.MainModule.Runtime.ToString().Replace("Net_", "").Replace("_", ".");
            return new Version(runtime);
        }
    }
}
