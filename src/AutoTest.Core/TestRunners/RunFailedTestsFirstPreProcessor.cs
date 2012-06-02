using System;
using System.Diagnostics;
using AutoTest.Messages;
using AutoTest.Core.Messaging.MessageConsumers;
using AutoTest.Core.Caching.RunResultCache;
using System.Collections.Generic;
namespace AutoTest.Core.TestRunners
{
	public class RunFailedTestsFirstPreProcessor : IPreProcessTestruns
	{
		private IRunResultCache _resultCache;
		
		public RunFailedTestsFirstPreProcessor(IRunResultCache resultCache)
		{
			_resultCache = resultCache;
		}
		
		public Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> FetchWrapper(Action<AutoTest.TestRunners.Shared.Targeting.Platform, Version, Action<ProcessStartInfo, bool>> wrapper)
		{
			return wrapper;
		}

        public PreProcessedTesRuns PreProcess(PreProcessedTesRuns preProcessed)
		{
            var details = preProcessed.RunInfos;
			foreach (var info in details)
			{
				info.AddTestsToRun(getTestsFor(info, _resultCache.Failed));
                info.AddTestsToRun(getTestsFor(info, _resultCache.Ignored));
				info.ShouldOnlyRunSpcifiedTestsFor(TestRunner.Any);
                info.ShouldRerunAllTestWhenFinishedFor(TestRunner.Any);
			}
            return new PreProcessedTesRuns(details);
		}

		public void RunFinished (TestRunResults[] results)
		{
		}
		
		private TestToRun[] getTestsFor(RunInfo info, TestItem[] cachedTests)
		{
			var tests = new List<TestToRun>();
			foreach (var failed in cachedTests)
			{
				if (failed.Key.Equals(info.Assembly))
					tests.Add(new TestToRun(TestRunner.Any, failed.Value.Name));
			}
			return tests.ToArray();
		}
	}
}

