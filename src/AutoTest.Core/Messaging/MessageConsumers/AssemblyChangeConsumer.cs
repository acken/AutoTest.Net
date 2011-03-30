using System;
using AutoTest.Core.TestRunners;
using System.Collections.Generic;
using AutoTest.Core.DebugLog;
using AutoTest.Messages;
using System.Threading;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class AssemblyChangeConsumer : IOverridingConsumer<AssemblyChangeMessage>
	{
		private ITestRunner[] _testRunners;
		private IMessageBus _bus;
		private IDetermineIfAssemblyShouldBeTested _testAssemblyValidator;
        private IPreProcessTestruns[] _preProcessors;
        private ILocateRemovedTests _removedTestLocator;
        private bool _isRunning = false;
        private bool _exit = false;
        private List<RunInfo> _abortedTestRuns = new List<RunInfo>();

        public bool IsRunning { get { return _isRunning; } }

        public AssemblyChangeConsumer(ITestRunner[] testRunners, IMessageBus bus, IDetermineIfAssemblyShouldBeTested testAssemblyValidator, IPreProcessTestruns[] preProcessors, ILocateRemovedTests removedTestLocator)
		{
			_testRunners = testRunners;
			_bus = bus;
			_testAssemblyValidator = testAssemblyValidator;
            _preProcessors = preProcessors;
            _removedTestLocator = removedTestLocator;
		}
		
		#region IConsumerOf[AssemblyChangeMessage] implementation
		public void Consume (AssemblyChangeMessage message)
		{
            _isRunning = true;
            var runReport = new RunReport();
            try
            {
			    informParticipants(message);
                var runInfos = getRunInfos(message);
                runInfos = preProcess(runInfos);
                runInfos = new TestRunInfoMerger(runInfos).MergeWith(_abortedTestRuns).ToArray();
                foreach (var runner in _testRunners)
                {
                    runTest(runner, runInfos, runReport);
                    if (_exit)
                    {
                        _abortedTestRuns.Clear();
                        _abortedTestRuns.AddRange(runInfos);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                var result = new TestRunResults("", "", false, TestRunner.Any, new TestResult[] { new TestResult(TestRunner.Any, TestRunStatus.Failed, "AutoTest.Net internal error", ex.ToString()) });
                _bus.Publish(new TestRunMessage(result));
            }
            _bus.Publish(new RunFinishedMessage(runReport));
            if (!_exit)
                _abortedTestRuns.Clear();
            _exit = false;
            _isRunning = false;
		}

        public void Terminate()
        {
            if (!_isRunning)
                return;
            Debug.WriteDebug("Initiating termination of current run");
            _exit = true;
            while (_isRunning)
                Thread.Sleep(10);
        }

		#endregion

        private RunInfo[] preProcess(RunInfo[] runInfos)
        {
            foreach (var preProcessor in _preProcessors)
                runInfos = preProcessor.PreProcess(runInfos);
            return runInfos;
        }
		private RunInfo[] getRunInfos(AssemblyChangeMessage message)

        {
            var runInfos = new List<RunInfo>();
            foreach (var file in message.Files)
            {
                var runInfo = new RunInfo(null);
                runInfo.SetAssembly(file.FullName);
                runInfos.Add(runInfo);
            }
            return runInfos.ToArray();
        }

		private void informParticipants(AssemblyChangeMessage message)
		{
			Debug.ConsumingAssemblyChangeMessage(message);
            _bus.Publish(new RunStartedMessage(message.Files));
		}

        private void runTest(ITestRunner runner, RunInfo[] runInfos, RunReport report)
		{
			var testRunInfos = new List<TestRunInfo>();
			foreach (var runInfo in runInfos)
			{
				if (_testAssemblyValidator.ShouldNotTestAssembly(runInfo.Assembly))
					return;
                if (runner.CanHandleTestFor(runInfo.Assembly))
                {
                    testRunInfos.Add(runInfo.CloneToTestRunInfo());
                    _bus.Publish(new RunInformationMessage(InformationType.TestRun, "", runInfo.Assembly, runner.GetType()));
                }
			}
            if (testRunInfos.Count == 0)
				return;
            var results = runner.RunTests(testRunInfos.ToArray(), () => { return _exit; });
            if (_exit)
                return;
			mergeReport(results, report, testRunInfos.ToArray());
            reRunTests(runner, report, testRunInfos);
		}

        private void reRunTests(ITestRunner runner, RunReport report, List<TestRunInfo> testRunInfos)
        {
            var rerunInfos = new List<TestRunInfo>();
            foreach (var info in testRunInfos)
            {
                if (info.RerunAllTestWhenFinishedForAny())
                    rerunInfos.Add(new TestRunInfo(info.Project, info.Assembly));
            }
            if (rerunInfos.Count > 0)
            {
                Debug.WriteDebug("Rerunning all tests for runner " + runner.GetType().ToString());
                var results = runner.RunTests(testRunInfos.ToArray(), () => { return _exit; });
                mergeReport(results, report, testRunInfos.ToArray());
            }
        }
		
		private void mergeReport(TestRunResults[] results, RunReport report, TestRunInfo[] runInfos)
		{
            var modifiedResults = new List<TestRunResults>();
			foreach (var result in results)
			{
                var modified = _removedTestLocator.SetRemovedTestsAsPassed(result, runInfos);
	            report.AddTestRun(
                    modified.Project,
                    modified.Assembly,
                    modified.TimeSpent,
                    modified.Passed.Length,
                    modified.Ignored.Length,
                    modified.Failed.Length);
                _bus.Publish(new TestRunMessage(modified));
                modifiedResults.Add(modified);
			}
            informPreProcessor(modifiedResults.ToArray());
		}

        private void informPreProcessor(TestRunResults[] results)
        {
            foreach (var preProcess in _preProcessors)
                preProcess.RunFinished(results);
        }
	}
}

