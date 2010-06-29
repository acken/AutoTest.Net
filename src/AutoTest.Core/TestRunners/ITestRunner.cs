using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core.TestRunners
{
    public interface ITestRunner
    {
        TestRunResults RunTests(Project project, string assemblyName);
    }
}