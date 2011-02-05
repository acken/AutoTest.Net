using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using AutoTest.TestRunners.Shared.Targeting;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public class AssemblyReader : IAssemblyReader
    {
        public Version GetTargetFramework(string assembly)
        {
            try
            {
                var asm = AssemblyDefinition.ReadAssembly(assembly);
                var runtime = asm.MainModule.Runtime.ToString().Replace("Net_", "").Replace("_", ".");
                return new Version(runtime);
            }
            catch
            {
                return new Version();
            }
        }

        public Platform GetPlatform(string assembly)
        {
            try
            {
                var asm = AssemblyDefinition.ReadAssembly(assembly);
                var architecture = asm.MainModule.Architecture;
                if (architecture == TargetArchitecture.I386)
                    return Platform.x86;
                if (architecture == TargetArchitecture.AMD64)
                    return Platform.AnyCPU;
                if (architecture == TargetArchitecture.IA64)
                    return Platform.AnyCPU;
                return Platform.Unknown;
            }
            catch
            {
                return Platform.Unknown;
            }
        }

        public IEnumerable<string> GetReferences(string assembly)
        {
            try
            {
                var ad = AssemblyDefinition.ReadAssembly(assembly);
                var references = ad.MainModule.AssemblyReferences;
                var names = new List<string>();
                foreach (var reference in references)
                    names.Add(reference.Name);
                return names;
            }
            catch
            {
            }
            return new string[] { };
        }
    }
}
