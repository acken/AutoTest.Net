using System;
using AutoTest.Messages;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.RunResultCache;
using System.Collections.Generic;
namespace AutoTest.Core.TestRunners
{
	class RunFailedTestsFirstPreProcessor : IPreProcessTestruns
	{
		private IRunResultCache _resultCache;
		
		public RunFailedTestsFirstPreProcessor(IRunResultCache resultCache)
		{
			_resultCache = resultCache;
		}
		
		public void PreProcess (RunInfo[] details)
		{
			foreach (var info in details)
			{
				info.AddTestsToRun(TestRunner.All, getTestsFor(info, _resultCache.Failed));
                info.AddTestsToRun(TestRunner.All, getTestsFor(info, _resultCache.Ignored));
				info.ShouldOnlyRunSpcifiedTestsFor(TestRunner.All);
                info.ShouldRerunAllTestWhenFinishedFor(TestRunner.All);
			}
		}

		public void RunFinished (TestRunResults[] results)
		{
		}
		
		private string[] getTestsFor(RunInfo info, TestItem[] cachedTests)
		{
			var tests = new List<string>();
			foreach (var failed in cachedTests)
			{
				if (failed.Key.Equals(info.Assembly))
					tests.Add(failed.Value.Name);
			}
			return tests.ToArray();
		}
	}
}

