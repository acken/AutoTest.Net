using System;
using AutoTest.Core.TestRunners;
using System.Collections.Generic;
using AutoTest.Core.DebugLog;
using AutoTest.Messages;
namespace AutoTest.Core.Messaging.MessageConsumers
{
	public class AssemblyChangeConsumer : IConsumerOf<AssemblyChangeMessage>
	{
		private ITestRunner[] _testRunners;
		private IMessageBus _bus;
		private IDetermineIfAssemblyShouldBeTested _testAssemblyValidator;
        private IPreProcessTestruns[] _preProcessors;

        public AssemblyChangeConsumer(ITestRunner[] testRunners, IMessageBus bus, IDetermineIfAssemblyShouldBeTested testAssemblyValidator, IPreProcessTestruns[] preProcessors)
		{
			_testRunners = testRunners;
			_bus = bus;
			_testAssemblyValidator = testAssemblyValidator;
            _preProcessors = preProcessors;
		}
		
		#region IConsumerOf[AssemblyChangeMessage] implementation
		public void Consume (AssemblyChangeMessage message)
		{
			informParticipants(message);
			var runReport = new RunReport();
            var runInfos = getRunInfos(message);
            preProcess(runInfos);
			foreach (var runner in _testRunners)
                runTest(runner, runInfos, runReport);
			_bus.Publish(new RunFinishedMessage(runReport));
		}
		#endregion

        private void preProcess(RunInfo[] runInfos)
        {
            foreach (var preProcessor in _preProcessors)
                runInfos = preProcessor.PreProcess(runInfos);
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
                    var testRunInfo = new TestRunInfo(null, runInfo.Assembly);
                    testRunInfo.AddTestsToRun(runInfo.GetTests());
                    testRunInfos.Add(testRunInfo);
                }
			}
            if (testRunInfos.Count == 0)
				return;
			var results = runner.RunTests(testRunInfos.ToArray());
			mergeReport(results, report);
		}
		
		private void mergeReport(TestRunResults[] results, RunReport report)
		{
			foreach (var result in results)
			{
	            report.AddTestRun(
	                result.Project,
	                result.Assembly,
	                result.TimeSpent,
	                result.Passed.Length,
	                result.Ignored.Length,
	                result.Failed.Length);
	            _bus.Publish(new TestRunMessage(result));
			}
		}
	}
}

