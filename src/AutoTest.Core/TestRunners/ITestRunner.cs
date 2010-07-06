using AutoTest.Core.Caching.Projects;
namespace AutoTest.Core.TestRunners
{
    public interface ITestRunner
    {
        bool CanHandleTestFor(ProjectDocument document);
        TestRunResults RunTests(Project project, string assemblyName);
    }
}