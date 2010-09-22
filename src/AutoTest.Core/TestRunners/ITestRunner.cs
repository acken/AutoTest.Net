using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;
namespace AutoTest.Core.TestRunners
{
    public interface ITestRunner
    {
        bool CanHandleTestFor(ProjectDocument document);
        TestRunResults[] RunTests(TestRunInfo[] runInfos);
    }
}