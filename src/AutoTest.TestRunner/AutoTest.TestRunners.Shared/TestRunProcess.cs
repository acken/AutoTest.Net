using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.TestRunners.Shared.Options;
using System.Threading;
using AutoTest.TestRunners.Shared.Results;
using AutoTest.TestRunners.Shared.Targeting;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Shared
{
    public class TestRunProcess : AutoTest.TestRunners.Shared.ITestRunProcess
    {
        private static List<TestResult> _results = new List<TestResult>();
        private IAssemblyReader _locator;
        private ITestRunProcessFeedback _feedback = null;
        private bool _runInParallel = false;
        private Func<bool> _abortWhen = null;
		private bool _activateProfiling = false;
		private string _profilerLogFile = null;

        public static void AddResults(IEnumerable<TestResult> results)
        {
            lock (_results)
            {
                _results.AddRange(results);
            }
        }

        public TestRunProcess()
        {
            _locator = new AssemblyReader();
        }

        public TestRunProcess(ITestRunProcessFeedback feedback)
        {
            _locator = new AssemblyReader();
            _feedback = feedback;
        }

        public TestRunProcess RunParallel()
        {
            _runInParallel = true;
            return this;
        }

        public TestRunProcess AbortWhen(Func<bool> abortWhen)
        {
            _abortWhen = abortWhen;
            return this;
        }
		
		public TestRunProcess ActivateProfilingFor(string logFileName)
		{
			_profilerLogFile = logFileName;
			_activateProfiling = true;
			return this;
		}

        public IEnumerable<TestResult> ProcessTestRuns(RunOptions options)
        {
            _results = new List<TestResult>();
            var workers = new List<Thread>();
            var testRuns = getTargetedRuns(options);
            foreach (var target in testRuns)
            {
                var process = new TestProcess(target, _feedback);
                if (_runInParallel)
                    process.RunParallel();
				if (_activateProfiling)
					process.ActivateProfilingFor(_profilerLogFile);
                process.AbortWhen(_abortWhen);
                var thread = new Thread(new ThreadStart(process.Start));
                thread.Start();
                workers.Add(thread);
            }
            foreach (var worker in workers)
                worker.Join();
            return _results;
        }

        private IEnumerable<TargetedRun> getTargetedRuns(RunOptions options)
        {
            var assembler = new TargetedRunAssembler(options, _locator);
            return assembler.Assemble();
        }
    }
}
