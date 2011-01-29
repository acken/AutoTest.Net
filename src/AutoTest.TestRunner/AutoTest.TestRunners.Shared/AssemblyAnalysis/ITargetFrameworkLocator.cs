using System;
namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public interface ITargetFrameworkLocator
    {
        Version Locate(string assembly);
    }
}
