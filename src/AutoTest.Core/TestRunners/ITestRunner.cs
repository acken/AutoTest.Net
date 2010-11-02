using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;

namespace AutoTest.Core.TestRunners
{
    public interface ITestRunner
    {
        bool CanHandleTestFor(ProjectDocument document);
		bool CanHandleTestFor(string assembly);
        TestRunResults[] RunTests(TestRunInfo[] runInfos);
    }
}