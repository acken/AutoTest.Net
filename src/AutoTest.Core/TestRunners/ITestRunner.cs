using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.FileSystem;
using AutoTest.Messages;
using System;
using System.Diagnostics;

namespace AutoTest.Core.TestRunners
{
    public interface ITestRunner
    {
        bool CanHandleTestFor(string assembly);
        TestRunResults[] RunTests(TestRunInfo[] runInfos, Action<Action<ProcessStartInfo>> processWrapper, Func<bool> abortWhen);
    }
}