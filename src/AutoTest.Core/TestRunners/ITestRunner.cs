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
		void Prepare(
			string[] assemblies,
			Action<AutoTest.TestRunners.Shared.Targeting.Platform,
				   Version,
				   Action<ProcessStartInfo, bool>> processWrapper,
				   Func<bool> abortWhen);
		void LoadAssemblies();
        TestRunResults[] RunTests(TestRunInfo[] runInfos);
    }
}
