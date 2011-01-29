using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public class AssemblyParser : IAssemblyParser
    {
        public Version GetTargetFramework(string assembly)
        {
            var asm = AssemblyDefinition.ReadAssembly(assembly);
            var runtime = asm.MainModule.Runtime.ToString().Replace("Net_", "").Replace("_", ".");
            return new Version(runtime);
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
