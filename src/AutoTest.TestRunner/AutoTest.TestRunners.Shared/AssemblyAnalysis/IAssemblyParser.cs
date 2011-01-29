using System;
using System.Collections.Generic;
namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public interface IAssemblyParser
    {
        Version GetTargetFramework(string assembly);
        IEnumerable<string> GetReferences(string assembly);
    }
}
