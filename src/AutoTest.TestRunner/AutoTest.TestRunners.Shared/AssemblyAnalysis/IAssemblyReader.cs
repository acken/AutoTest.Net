using System;
using System.Collections.Generic;
using AutoTest.TestRunners.Shared.Targeting;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public interface IAssemblyReader
    {
        Version GetTargetFramework(string assembly);
        Platform GetPlatform(string assembly);
        IEnumerable<string> GetReferences(string assembly);
    }
}
